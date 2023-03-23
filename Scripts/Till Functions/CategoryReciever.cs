using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryReciever : MonoBehaviour
{
    public CategoryReciever instance;

    [Header("Catagories")]
    public List<Category> categoriesRecieved = new List<Category>();
    public bool allCategoriesRecieved;
    public List<KeyValuePair<long, int>> categoryItemReturn = new List<KeyValuePair<long, int>>(); //item id, item pos
    public bool allCategoryItemsReceived;
    public Item itemReturn;

    [Header("References")]
    private ClientController clientController;
    private Client client;

    // Start is called before the first frame update
    void Start()
    {
        //Sets the script as an instance
        instance = this;
        //Creates references to other scripts
        clientController = FindObjectOfType<ClientController>();
        client = clientController.GetComponent<Client>();
    }

    //Requests Categories from the server
    public IEnumerator RequestCategories()
    {
        //Creates the initial category request
        string q_toSend = "&CATEGORYDBR|" + clientController.instance.categoryNumber.ToString();
        client.instance.toSend.AddLast(q_toSend);

        //Waits for all categories to be sent back
        yield return new WaitUntil(() => allCategoriesRecieved);

        //Iterates through all recieved categories
        foreach (Category cat in categoriesRecieved)
        {
            //Creates a secondary request to the server
            string q2_toSend = "&CATEGORYITEMSDBR|" + cat.categoryID.ToString();
            categoryItemReturn = new List<KeyValuePair<long, int>>();
            allCategoryItemsReceived = false;
            client.instance.toSend.AddLast(q2_toSend);

            //Waits for the items to be returned
            yield return new WaitUntil(() => allCategoryItemsReceived);

            //Iterates through the retruned items
            for (int i = 0; i < categoryItemReturn.Count; i++)
            {
                //Creates a third request to the server
                string q3_toSend = "&CATEGORYITEMNAMEDBR|" + categoryItemReturn[i].Key.ToString();
                client.instance.toSend.AddLast(q3_toSend);

                //Waits for the item name to be returned
                yield return new WaitUntil(() => itemReturn != null);

                //assigns the item name 
                cat.itemsInCategory[categoryItemReturn[i].Value] = itemReturn;
                itemReturn = null;
            }
        }
        //Passes the categories that were recieved to the client controller and resets, ready for another request
        clientController.instance.catagories = categoriesRecieved;
        categoriesRecieved = new List<Category>();
        clientController.instance.InitlaiseCategoryButtons();
    }
}
