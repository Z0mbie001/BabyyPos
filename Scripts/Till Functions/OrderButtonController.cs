using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderButtonController : MonoBehaviour
{
    [Header("Text field")]
    public Text productName;
    public Text productQuantity;

    [Header("Items")]
    public Item item;

    [Header("References")]
    private ClientController clientController;

    // Start is called before the first frame update
    void Start()
    {
        clientController = FindObjectOfType<ClientController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Updates the button's details
    public void UpdateButton()
    {
        Start();
        if (!clientController.instance.itemsInOrder.ContainsKey(item))
        {
            clientController.instance.orderButtons.Remove(this);
            Destroy(gameObject);
            return;
        }
        if(clientController.instance.selectedItem == item)
        {
            gameObject.GetComponent<Image>().color = new Color(210, 210, 210);
            //gameObject.GetComponent<Button>().Select();
        }
        else
        {
            gameObject.GetComponent<Image>().color = Color.white;
        }
        productName.text = item.name;
        productQuantity.text = clientController.instance.itemsInOrder[item].ToString();
    }

    //When the item the button represents is selected
    public void SelectItem()
    {
        clientController.instance.selectedItem = item;
        clientController.instance.UpdateOrderButtons();
    }
}
