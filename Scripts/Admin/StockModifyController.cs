using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockModifyController : MonoBehaviour
{
    [Header("Input boxes")]
    public InputField nameInput;
    public InputField priceInput;
    public InputField typeInput;

    [Header("Item value")]
    public Item item;

    [Header("GameObjects")]
    public GameObject panel;
    public GameObject confirmationPanel;

    [Header("References")]
    private Client client;

    // Start is called before the first frame update
    void Start()
    {
        client = FindObjectOfType<Client>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Run when a UI button is pressed
    public void ConfirmButtonPress()
    {
        //Debug.Log("Confirm changes");
        string name, type, price;
        //Validating the name input
        if (nameInput.text == "")
        {
            name = item.name;
        }
        else
        {
            name = nameInput.text;
        }
        //Validating the Price input
        if (priceInput.text == "")
        {
            price = item.price.ToString();
        }
        else
        {
            price = priceInput.text;
        }
        //Validating the Type input
        if (typeInput.text == "")
        {
            type = item.type.ToString();
        }
        else
        {
            type = typeInput.text;
        }
        //Create server request
        string q_toSend = "&STOCKWR|" + item.id + "|" + name + "|" + price + "|" + type;
        client.instance.toSend.AddLast(q_toSend);
        ClosePanel();
    }

    //Closes the panel
    public void ClosePanel()
    {
        Destroy(panel);
    }

    //Opens another panel
    public void OpenDeletePanel()
    {
        confirmationPanel.SetActive(true);
    }

    //Requests an item to be deleted
    public void DeleteItem()
    {
        //Creates a server request
        string q_toSend = "&STOCKDELETER|" + item.id;
        client.instance.toSend.AddLast(q_toSend);
        ClosePanel();
    }

    //Closes the delete panel
    public void CloseDeletePanel()
    {
        confirmationPanel.SetActive(false);
    }
}
