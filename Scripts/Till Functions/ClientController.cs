using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ClientController : MonoBehaviour
{
    public ClientController instance;
    [Header("Till Details")]
    public string tillName = "UnknownTill";
    public int transactionNumber;
    public int categoryNumber = 0;

    [Header("Order Details")]
    public Dictionary<Item, int> itemsInOrder = new Dictionary<Item, int>();
    public float orderSubTotal;
    public List<OrderButtonController> orderButtons;
    public Item selectedItem;
    public Text subTotalText;
    public InputField quantityInput;
    public InputField productIDInput;

    [Header("Holders and Prefabs")]
    public GameObject orderButtonPrefab;
    public GameObject orderContentHolder;
    public GameObject categoryPrefab;
    public GameObject categoryHolder;
    public GameObject categoryItemHolder;

    [Header("Catagories")]
    public List<Category> catagories = new List<Category>();
    public Category activeCategory;
    public List<GameObject> activeCategoryItemButtons = new List<GameObject>();

    [Header("Staff")]
    public StaffMember activeStaffMember;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        UpdateOrderButtons();
        CalculateSubTotal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Adds an item to the order
    public void AddItemToOrder(Item itemToAdd)
    {
        if (!CheckObjectInArray<Item>(itemToAdd, itemsInOrder.Keys.ToArray<Item>()))
        {
            itemsInOrder.Add(itemToAdd, 1);
            OrderButtonController newButton = Instantiate(orderButtonPrefab, orderContentHolder.transform).GetComponent<OrderButtonController>();
            orderButtons.Add(newButton);
            newButton.item = itemToAdd;
        }
        else
        {
            itemsInOrder[itemToAdd] += 1;
        }
        UpdateOrderButtons();
        CalculateSubTotal();
    }

    //Calculate the Sub-total of the order
    public void CalculateSubTotal()
    {
        orderSubTotal = 0;
        foreach(KeyValuePair<Item, int> kvp in itemsInOrder)
        {
            orderSubTotal += kvp.Key.price * kvp.Value;
        }
        //orderSubTotal = (float)(System.Math.Round(orderSubTotal, 3));
        subTotalText.text = "Sub-Total: £" + orderSubTotal.ToString("0.00");
    }

    //Updates all the UI buttons in the order
    public void UpdateOrderButtons()
    {
        if (orderButtons.Count > 0)
        {
            foreach (OrderButtonController obc in orderButtons.ToArray())
            {
                obc.UpdateButton();
            }
            if (selectedItem != null)
            {
                quantityInput.text = itemsInOrder[selectedItem].ToString();
            }
            else
            {
                quantityInput.text = "";
            }
        }
        productIDInput.text = string.Empty;
    }

    //Removes and item from the order
    public void RemoveItemFromOrder()
    {
        if(selectedItem == null)
        {
            return;
        }
        
        /*foreach(OrderButtonController obc in orderButtons)
        {
            if(obc.item == selectedItem)
            {
                Destroy(obc.gameObject);
                orderButtons.Remove(obc);
            }
        }*/
        itemsInOrder.Remove(selectedItem);
        selectedItem = null;
        UpdateOrderButtons();
        CalculateSubTotal();
    }

    //Increases the quantity of an item in the order
    public void IncreaseQuantity()
    {
        if(selectedItem == null)
        {
            return;
        }
        int.TryParse((itemsInOrder[selectedItem] + 1).ToString(), out int tempQuantity);
        if(tempQuantity < itemsInOrder[selectedItem])
        {
            return;
        }
        else
        {
            itemsInOrder[selectedItem] = tempQuantity;
            FindObjectOfType<Client>().CreateErrorPopup("Quanity exceeded maximum limit");
        }
        UpdateOrderButtons();
        CalculateSubTotal();
    }    
    
    //Decreases the quantity of an item in the order
    public void DecreaseQuantity()
    {
        if(selectedItem == null)
        {
            return;
        }
        if (itemsInOrder[selectedItem] - 1 < 1)
        {
            return;
        }
        itemsInOrder[selectedItem] -= 1;
        UpdateOrderButtons();
        CalculateSubTotal();
    }

    //Sets a manual quantity for an item in the order
    public void ManualQuantity()
    {
        int quant;
        if(quantityInput.text != "" && selectedItem != null)
        {
            Int32.TryParse(quantityInput.text, out quant);
            if(quant == 0)
            {
                quant = itemsInOrder[selectedItem];
            }
            /*
             * O tried to break the code, this was one point of failure. 
             * Changed int to 'TryParse' rather than direct conversion
             * Also checked that value is never equal to 0
             */
            if (quant > 0)
            {
                itemsInOrder[selectedItem] = quant;
                quantityInput.text = "";
            }
            else
            {
                if(activeStaffMember.permissionLevel >= 3)
                {
                    Debug.Log("Returns Activated");
                    itemsInOrder[selectedItem] = quant;
                    quantityInput.text = "";
                }
                else
                {
                    Debug.Log("Not sufficent permissions to refund");
                    quantityInput.text = "";
                    return;
                }
            }
        }
        UpdateOrderButtons();
        CalculateSubTotal();
    }

    //Instanciates the Category buttons
    public void InitlaiseCategoryButtons()
    {
        foreach(Category cat in catagories)
        {
            GameObject newCategoryButton = Instantiate(categoryPrefab, categoryHolder.transform);
            CategoryButtonController catButtonController = newCategoryButton.GetComponent<CategoryButtonController>();
            catButtonController.buttonCategory = cat;
            catButtonController.UpdateButton();
        }
    }

    //Creates a button for an item in the category
    public void CreateCategoryItemButtons()
    {
        foreach(GameObject go in activeCategoryItemButtons)
        {
            Destroy(go);
        }
        activeCategoryItemButtons.Clear();
        if(activeCategory == null)
        {
            activeCategory = catagories[0];
        }
        foreach(Item item in activeCategory.itemsInCategory)
        {
            if (item != null)
            {


                GameObject newButton = Instantiate(FindObjectOfType<StockLookup>().instance.SLUButton, categoryItemHolder.transform);
                SLUButtonController sLUButtonController = newButton.GetComponent<SLUButtonController>();
                sLUButtonController.item = item;
                sLUButtonController.SetText();
                activeCategoryItemButtons.Add(newButton);
            }
        }
    }

    //Requests for an item to be added to the order
    public void SubmitBarcodeNumber()
    {
        long.TryParse(productIDInput.text, out long productId);
        //Create a request to the server
        string q_toSend = "&DIRECTSTOCKDBR|" + productId;
        FindObjectOfType<Client>().instance.toSend.AddLast(q_toSend);
    }

    public bool CheckObjectInArray<type>(type item, type[] array)
    {
        foreach(type typeItem in array)
        {
            if(typeItem.Equals(item) && typeItem != null)
            {
                return true;
            }
        }
        return false;
    }

    /*
     * ISSUE: Items not increasing quantity
     * DESC: When multiple of the same item are added the quanity of the first should increase, this wasnt happening
     * STATUS: Resolved
     * SOLUTION: Programmed a custom loop to check if the item is in the dictonary already
     */
}
