using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StockLookup : MonoBehaviour
{
    public StockLookup instance;

    [Header("Items")]
    public List<Item> returnedItems = new List<Item>();
    public bool allItemsReturned;

    [Header("GameObject")]
    public GameObject stockLUPanel;
    public GameObject SLUButton;
    public GameObject SLUContent;
    private List<GameObject> activeSLUButtons = new();

    [Header("InputFields")]
    public InputField productNameInput;
    public InputField productIDInput;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;    
    }

    //used to request stock from the server
    public void RequestStockFromServer()
    {
        RemoveSLUButtons();
        //get the input data
        string idToRequest = productIDInput.text == "" ? "0" : productIDInput.text;
        string nameToRequest = productNameInput.text.ToString();
        productIDInput.text = "";
        productNameInput.text = "";
        //create a request in the correct syntax
        string requestToSend = "&STOCKLUDBR|" + idToRequest + "|" + nameToRequest;
        Debug.Log(requestToSend);
        //send the request to the server
        FindObjectOfType<Client>().instance.Send(requestToSend);
        //create the SLU Buttons
        StartCoroutine(WaitForItems());
    }

    //opens the stock lookup panel
    public void OpenPanel()
    {
        stockLUPanel.gameObject.SetActive(true);
    }

    //Coloses the Stock Lookup Panel
    public void ClosePanel()
    {
        RemoveSLUButtons();
        productIDInput.text = "";
        productNameInput.text = "";
        stockLUPanel.gameObject.SetActive(false);
    }

    //instaciates buttons for the stock lookup panel
    public void CreateSLUButtons()
    {
        if(returnedItems.Count <= 0)
        {
           FindObjectOfType<Client>().CreateErrorPopup("No Items Returned");
            return;
        }
        foreach(Item item in returnedItems)
        {
            GameObject newButton = Instantiate(SLUButton, SLUContent.transform);
            activeSLUButtons.Add(newButton);
            SLUButtonController sluButtonController = newButton.GetComponent<SLUButtonController>();
            sluButtonController.item = item;
            sluButtonController.SetText();
        }
        returnedItems.Clear();
        allItemsReturned = false;
    }

    //Deletes the Stock Lookup Buttons
    public void RemoveSLUButtons()
    {
        if(activeSLUButtons.Count <= 0)
        {
            return;
        }
        while(activeSLUButtons.Count > 0) 
        {
                Destroy(activeSLUButtons[0].gameObject);
                activeSLUButtons.Remove(activeSLUButtons[0]);
        }
    }

    //waits for items to be returned
    IEnumerator WaitForItems()
    {
        yield return new WaitUntil(() => allItemsReturned);
        CreateSLUButtons();
        yield return null;
    }
}
