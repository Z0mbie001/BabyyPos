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
        if(idInput.text == "" || nameInput.text == "" || priceInput.text == "" || typeInput.text == "")
        {
            Debug.LogWarning("Missing data, cannot send request");
            return;
        }

        string q_toSend = "&STOCKWR|" + idInput.text + "|" + nameInput.text + "|" + priceInput.text + "|" + typeInput.text;
        FindObjectOfType<Client>().toSend.AddLast(q_toSend);
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
