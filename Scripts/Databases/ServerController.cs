using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ServerController : MonoBehaviour
{
    public ServerController instance;

    [Header("References")]
    private Server server;
    private DatabaseManager dbManager;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        server = FindObjectOfType<Server>();
        dbManager = FindObjectOfType<DatabaseManager>();
    }

    //Processes a stock lookup request
    public void StockLookupRequest(ServerClient client, long id, string itemName)
    {
        //reads stock items from the database
        List<Item> data = dbManager.instance.ReadValuesInStockTable(id, itemName);
        //Debug.Log("Length of data: " + data.Count);
        //cycle through each item
        foreach (Item item in data)
        {
            //send each item to the client that requested the lookup
            string toSend = "%STOCKLURT|" + item.id.ToString() + "|" + item.name + "|" + item.price.ToString() + "|" + item.type.ToString();
            server.instance.ToSend.AddLast((toSend, client));
        }
        //send the closing statement for the request
        server.instance.ToSend.AddLast(("%STOCKLURT|~END", client));
    }

    //Processes a category request
    public void CategoryRequest(ServerClient client, int id)
    {
        List<Category> data = dbManager.instance.ReadValuesInCategoryTable(id);
        foreach(Category cat in data)
        {
            string catColour = UnityEngine.ColorUtility.ToHtmlStringRGBA(cat.categoryColour);
            string toSend = "%CATEGORYRT|" + cat.categoryID + "|" + cat.categoryName + "|" + catColour;
            server.instance.ToSend.AddLast((toSend, client));
        }
        server.instance.ToSend.AddLast(("%CATEGORYRT|~END", client));
    }

    //Processes a category item request
    public void CategoryItemRequest(ServerClient client, int id)
    {
        List<KeyValuePair<long, int>> data = dbManager.instance.ReadValuesInCategoryItemTable(id); //ID, Pos
        foreach(KeyValuePair<long, int> kvp in data)
        {
            string toSend = "%CATEGORYITEMRT|" + kvp.Key.ToString() + "|" + kvp.Value.ToString();
            server.instance.ToSend.AddLast((toSend, client));
        }
        server.instance.ToSend.AddLast(("%CATEGORYITEMRT|~END", client));
    }

    //Processes a request for category item data
    public void CategoryItemDataRequest(ServerClient client, long id)
    {
        List<Item> data = dbManager.instance.ReadValuesInStockTable(id, "");
        foreach(Item item in data)
        {
            string toSend = "%CATEGORYITEMNAMERT|" + item.id + "|" + item.name + "|" + item.price + "|" + item.type;
            server.instance.ToSend.AddLast((toSend, client));
        }
        //server.instance.toSend.AddLast(("%CATEGORYITEMNAMERT|~END", client));

    }

    //Processes a staff login request
    public void StaffLoginRequest(ServerClient client, int staffID)
    {
        List<StaffMember> data = dbManager.instance.ReadStaffMembersInTable(staffID, "", "");
        foreach(StaffMember sm in data)
        {
            string toSend = "%STAFFLOGINRT|" + sm.staffID.ToString() + "|" + sm.lastName + "|" + sm.firstName + "|" + sm.dateOfBirth + "|" + sm.startDate + "|" + sm.endDate + "|" + sm.permissionLevel.ToString();
            server.instance.ToSend.AddLast((toSend, client));
        }
        server.instance.ToSend.AddLast(("%STAFFLOGINRT|~END", client));
    }

    //Processes a transaction write request
    public void WriteTransactionData(ServerClient client, Transaction transToAdd)
    {
        dbManager.instance.InsertValuesIntoTransTable(transToAdd.transactionID, transToAdd.transactionDateTime, transToAdd.transactionAmount, transToAdd.paymentType, transToAdd.staffID);
        //server.instance.toSend.AddLast(("%TRANSACTIONWRITERT|~COMPLETE", client));
    }    
    
    //Processes a transaction item write request
    public void WriteTransactionItemData(ServerClient client, long transId, long itemID, int quantity, float itemPrice)
    {
        dbManager.instance.InsertValuesIntoTransItemTable(transId, itemID, quantity, itemPrice);
        //server.instance.toSend.AddLast(("%TRANSACTIONWRITERT|~COMPLETE", client));
    }

    //Processes an authorising staff member request
    public void AuthStaffMemberData(ServerClient client, int idToSearch)
    {
        List<StaffMember> data = dbManager.instance.ReadStaffMembersInTable(idToSearch, "", "");
        string toSend = "%AUTHSTAFFRT|" + data[0].staffID.ToString() + "|" + data[0].lastName + "|" + data[0].firstName + "|" + data[0].dateOfBirth + "|" + data[0].startDate + "|" + data[0].endDate + "|" + data[0].permissionLevel.ToString();
        server.instance.ToSend.AddLast((toSend, client));
    }

    //Processes a stock item write request
    public void WriteStockItem(ServerClient client, long id, string name, float price, int type)
    {
        dbManager.instance.InsertValueIntoStockTable(id, name, price, type);

    }

    //Processes a write staff member request
    public void WriteStaffMember(ServerClient client, int id, string firstName, string lastName, string dateOfBirth, string startDate, string endDate, int premssionsLv)
    {
        DateTime dob, startDateFormatted, endDateFormatted;
        DateTime.TryParse(dateOfBirth, out dob);
        DateTime.TryParse(startDate, out startDateFormatted);
        DateTime.TryParse(endDate, out endDateFormatted);
        StaffMember newStaffMember = new StaffMember(id, lastName, firstName, dob, startDateFormatted, endDateFormatted, premssionsLv);
        dbManager.InsertValuesIntoStaffTable(newStaffMember);
    }

    //Processes a direct stock request
    public void DirectStockRequest(ServerClient client, long id)
    {
        List<Item> data = new List<Item>();
        string toSend;
        data = dbManager.ReadValuesInStockTable(id, "");
        if(data.Count != 1)
        {
            toSend = "%DIRECTSTOCKRT|ERROR";
        }
        else
        {
            toSend = "%DIRECTSTOCKRT|" + data[0].id.ToString() + '|' + data[0].name + '|' + data[0].price.ToString() + '|' + data[0].type.ToString();
        }
        server.instance.ToSend.AddLast((toSend, client));
    }
}
