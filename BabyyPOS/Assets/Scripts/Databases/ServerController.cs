using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerController : MonoBehaviour
{
    public ServerController instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Processes a stock lookup request
    public void StockLookupRequest(ServerClient client, int id, string itemName)
    {
        //reads stock items from the database
        List<Item> data = FindObjectOfType<DatabaseManager>().instance.ReadValuesInStockTable(id, itemName);
        //cycle through each item
        foreach (Item item in data)
        {
            //send each item to the client that requested the lookup
            string toSend = "%STOCKLURT|" + item.id.ToString() + "|" + item.name + "|" + item.price + "|" + item.type.ToString();
            FindObjectOfType<Server>().instance.Send(toSend, client);
        }
        //send the closing statement for the request
        FindObjectOfType<Server>().instance.Send("%STOCKLURT|~END", client);
    }
}
