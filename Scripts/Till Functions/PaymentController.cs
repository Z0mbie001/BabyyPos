using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PaymentController : MonoBehaviour
{
    public PaymentController instance;

    [Header("GameObjects")]
    public GameObject paymentScreen;
    public GameObject cardPaymentScreen;
    public GameObject cashPaymentScreen;
    public GameObject ageVerificationPanel;
    public GameObject authorisationPanel;

    public GameObject paymentAmountPrefab;
    public GameObject paymentAmountHolder;
    public List<GameObject> paymentButtons;

    [Header("Int and float values")]
    public int paymentType;
    public float amountPaid;
    public float amountRemaining;

    [Header("Staff Members")]
    public StaffMember authorisingStaffMember;
    public StaffMember returnedStaffMember;

    [Header("Text Fields")]
    public Text totalAmountText;
    public Text paidText;
    public Text remainingText;

    [Header("Input Field")]
    public InputField cashAmountInput;
    public InputField authorisationInput;

    [Header("References")]
    private ClientController clientController;
    private Client client;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        clientController = FindObjectOfType<ClientController>();
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Opens the payment screen
    public void OpenPaymentScreen()
    {
        if (clientController.instance.itemsInOrder.ToArray().Length > 0)
        {
            paymentScreen.SetActive(true);
            amountRemaining = clientController.orderSubTotal;
            amountPaid = 0;
            UpdateText();
            authorisingStaffMember = clientController.activeStaffMember;
        }
        else
        {
            return;
        }
    }

    //Closes teh payment screen
    public void ClosePaymentScreen()
    {
        paymentScreen.SetActive(false);
        foreach(GameObject go in paymentButtons)
        {
            Destroy(go);
        }
        paymentButtons.Clear();
    }

    //Completes the transaction, sending detials to the database
    public void CompleteTransaction()
    {
        if(amountRemaining > 0 )
        {
            client.CreateErrorPopup("Not fully paid");
            return;
            //This is another error O pointed out, you can complete a transaction without paying for your items
        }
        Transaction transactionToSend = GetTransactionDetails();
        string toSend_transaction = "&TRANSACTIONDBWR|" + transactionToSend.transactionID.ToString() + "|" + transactionToSend.transactionDateTime.ToString("s") + "|" + transactionToSend.transactionAmount.ToString() + "|" + transactionToSend.paymentType.ToString() + "|" + transactionToSend.staffID.ToString();
        client.instance.toSend.AddLast(toSend_transaction);
        foreach((Item, int) item in transactionToSend.transactionItems)
        {
            string toSend_transactionItem = "&TRANSACTIONITEMDBWR|" + transactionToSend.transactionID.ToString() + "|" + item.Item1.id.ToString() + "|" + item.Item2.ToString() + "|" + item.Item1.price.ToString();
            client.instance.toSend.AddLast(toSend_transactionItem);
        }
        ClosePaymentScreen();
        ageVerificationPanel.SetActive(false);
        clientController.itemsInOrder.Clear();
        clientController.selectedItem = null;
        clientController.transactionNumber += 1;
        clientController.UpdateOrderButtons();
        paymentType = 0;
        clientController.CalculateSubTotal();
        FindObjectOfType<SaveLoadController>().instance.SaveData();
    }

    //Fetches the transaction's details
    Transaction GetTransactionDetails()
    {
        Transaction transDetails;
        string transID = "";
        for(int i = 0; i < clientController.instance.tillName.Length && i < 11; i++)
        {
            transID += Encoding.Default.GetBytes(clientController.instance.tillName.ToCharArray())[i].ToString();
        }
        transID += DateTime.Now.DayOfYear.ToString() + DateTime.Now.Year.ToString();
        transID += clientController.instance.transactionNumber;
        if(transID.Length > 18)
        {
            transID = transID.Substring(transID.Length - 17, transID.Length - 1);
        }
        DateTime currentDateTime = DateTime.Now;
        clientController.instance.UpdateOrderButtons();
        clientController.instance.CalculateSubTotal();
        float transAmount = clientController.instance.orderSubTotal;
        int staffID = authorisingStaffMember.staffID;
        List<(Item, int)> items = new();
        foreach (KeyValuePair<Item, int> item in clientController.instance.itemsInOrder)
        {
            items.Add((item.Key, item.Value));
        }
        long.TryParse(transID, out long transIDInt);
        transDetails = new Transaction(transIDInt, currentDateTime, transAmount, paymentType, staffID, items);
        return transDetails;
    }

    //Used to initiate a type of payment
    public void StartPayment(int typeOfPayment)
    {
        if(typeOfPayment == 1)
        {
            StartCoroutine(CardPayment());
            if (paymentType == 0)
            {
                
                paymentType = 1;
            }
            else
            {
                paymentType = 3;
            }
        }
        else if(typeOfPayment == 2)
        {
            CashPayment();
            if (paymentType == 0)
            {
                paymentType = 2;
            }
            else
            {
                paymentType = 3;
            }
        }
    }

    //Used when cash is being payed
    public void CashPayment()
    {
        float inputAmount = amountRemaining;
        cashPaymentScreen.SetActive(true);
        if (cashAmountInput.text != "")
        {
            float.TryParse(cashAmountInput.text, out inputAmount);
            if(inputAmount == 0)
            {
                cashAmountInput.text = "";
                paymentType = 0;
                client.CreateErrorPopup("Float lenght exceeded limit");
                return;
            }
            else
            {
                amountPaid += inputAmount;
                amountRemaining = clientController.orderSubTotal - amountPaid;
                GameObject newButton = Instantiate(paymentAmountPrefab, paymentAmountHolder.transform);
                PaymentButtonController payButtonController = newButton.GetComponent<PaymentButtonController>();
                payButtonController.amount = inputAmount;
                payButtonController.paymentName = "Cash";
                payButtonController.UpdateButtonDetails();
                cashAmountInput.text = "";
                paymentButtons.Add(newButton);
                UpdateText();
            }
        }
    }
    
    //Used to close the chas payment screen
    public void CloseCashPaymentScreen()
    {
        cashPaymentScreen.SetActive(false);
    }

    //Updates the UI text elements
    void UpdateText()
    {
        totalAmountText.text = "Order Total: �" + clientController.instance.orderSubTotal.ToString("0.00");
        paidText.text = "Amount Paid: � " + amountPaid.ToString("0.00");
        if (amountRemaining >= 0)
        {
            remainingText.text = "Amount Due: �" + amountRemaining.ToString("0.00");
        }
        else
        {
            remainingText.text = "Change Due: �" + Math.Abs(amountRemaining).ToString("0.00");
        }
    }

    //Used to process card payments
    IEnumerator CardPayment()
    {
        cardPaymentScreen.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        cardPaymentScreen.SetActive(false);
        GameObject newButton = Instantiate(paymentAmountPrefab, paymentAmountHolder.transform);
        PaymentButtonController payButtonController = newButton.GetComponent<PaymentButtonController>();
        payButtonController.amount = amountRemaining;
        payButtonController.paymentName = "Card";
        payButtonController.UpdateButtonDetails();
        paymentButtons.Add(newButton);
        amountPaid += amountRemaining;
        amountRemaining = 0;
        UpdateText();
    }

    //Checks if there are any restricted items
    public bool RestrictedItemCheck()
    {
        foreach(KeyValuePair<Item, int> kvp in clientController.instance.itemsInOrder)
        {
            if(kvp.Key.type == 5 || kvp.Key.type == 6)
            {
                return true;
            }
        }
        return false;
    }

    //Checks if the staff are over 18
    public bool StaffOver18()
    {
        if(DateTime.Now >= authorisingStaffMember.dateOfBirth.AddYears(18))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Verifies if an age check is required
    public void VerifyAge()
    {
        if(RestrictedItemCheck())
        {
            if (StaffOver18())
            {
                ageVerificationPanel.SetActive(true);
            }
            else
            {
                StartCoroutine(CheckNewAuth());   
            }
        }
        else
        {
            CompleteTransaction();
        }
    }

    //requestss a new staff member to authorise the payment
    IEnumerator CheckNewAuth()
    {
        authorisationPanel.SetActive(true);
        returnedStaffMember = null;
        yield return new WaitUntil(() => returnedStaffMember != null);
        Debug.Log("Server returned : " + returnedStaffMember.lastName);
        if(DateTime.Now >= returnedStaffMember.dateOfBirth.AddYears(18))
        {
            authorisingStaffMember = returnedStaffMember;
            authorisationPanel.SetActive(false);
            ageVerificationPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Staff member not authorised to sell restricted items");
            authorisationPanel.SetActive(false);
            yield return null;
        }
    }

    //Sends a request to the server for authorisation staff member
    public void OnAuthPress()
    {
        string toSend = "&AUTHSTAFFDBR|" + authorisationInput.text;
        authorisationInput.text = "";
        client.toSend.AddLast(toSend);
    }

    //Closes the authorisation panel
    public void CloseAuthPanel()
    {
        authorisationPanel.SetActive(false);
    }
}
