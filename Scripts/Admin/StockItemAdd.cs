using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockItemAdd : MonoBehaviour
{
    [Header("Input Values")]
    public InputField idInput;
    public InputField nameInput;
    public InputField priceInput;
    public InputField typeInput;

    //Activated when the confirm button is pressed
    public void OnConfirmButtonPress()
    {
        // Validate that none of the inputs are left blank
        if(idInput.text == "" || nameInput.text == "" || priceInput.text == "" || typeInput.text == "")
        {
            Debug.LogWarning("Missing data, cannot send request");
            return;
        }

        // Format a query for the server and add it to the send queue
        string q_toSend = "&STOCKWR|" + idInput.text + "|" + nameInput.text + "|" + priceInput.text + "|" + typeInput.text;
        FindObjectOfType<Client>().toSend.AddLast(q_toSend);

        //Reset the input fields
        idInput.text = "";
        nameInput.text = "";
        priceInput.text = "";
        typeInput.text = "";
        ClosePanel();
    }

    //Closes the panel
    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
