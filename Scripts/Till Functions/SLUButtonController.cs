using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SLUButtonController : MonoBehaviour
{
    [Header("Values")]
    public GameObject modifyPopup;
    public Item item;

    [Header("References")]
    private ClientController clientController;

    // Start is called before the first frame update
    void Start()
    {
        clientController = FindObjectOfType<ClientController>();
    }

    //Sets the text on the button
    public void SetText()
    {
        GetComponentInChildren<Text>().text = item.name;
    }

    //Activates when the button is pressed
    public void OnPress()
    {
        if (clientController == null)
        {
            GameObject popup = Instantiate(modifyPopup, FindObjectOfType<Canvas>().transform);
            popup.GetComponent<StockModifyController>().item = item;
        }
        else
        {
            clientController.instance.AddItemToOrder(item);
            FindObjectOfType<StockLookup>().instance.ClosePanel();
        }
    }
}
