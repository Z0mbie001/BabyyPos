using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockLookup : MonoBehaviour
{
    public StockLookup instance;

    public List<Item> returnedItems;
    public bool allItemsReturned;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //used to request stock from the server
    private void RequestStockFromServer(int idToRequest, string nameToRequest)
    {
        //create a request in the correct syntax
        string requestToSend = "&STOCKLUDBR|" + idToRequest + "|" + nameToRequest;
        //send the request to the server
        FindObjectOfType<Client>().instance.Send(requestToSend);
    }
}
