using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class Client : MonoBehaviour
{
    public Client instance;

    [Header("Client Details")]
    public string clientName = "Unknown";

    [Header("Connection Details")]
    public string hostIP = "192.168.1.";
    public int hostPort = 0;

    [Header("Client Infrastaucture")]
    private bool socketReady;
    public bool connected;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    public LinkedList<string> toSend = new();

    [Header("GameObjects")]
    public GameObject errorPopupPrefab;
    public GameObject errorPopupHolder;
    public GameObject hostPanel;

    [Header("References")]
    private ClientController cController;
    private CategoryReciever catReciever;
    private Encryption encrypter;
    private StockLookup stockLookup;
    private StaffLoginController stafflogin;

    //A proceudre that runs in the first frame
    private void Start()
    {
        //Sets the script as an instance
        instance = this;
        //Creates references to other scripts
        cController = FindObjectOfType<ClientController>();
        catReciever = FindObjectOfType<CategoryReciever>();
        encrypter = FindObjectOfType<Encryption>();
        stockLookup = FindObjectOfType<StockLookup>();
        stafflogin = FindObjectOfType<StaffLoginController>();
    }

    //Allows the client to connect to the server
    public void ConnectToServer()
    {
        Debug.Log("Trying to conect to the server");
        //if already connected, ignore this function
        if (socketReady)
        {
            Debug.Log("Connection Failed 1");
            return;
        }
        //Add the client Username
        if (GameObject.Find("TillNameInput").GetComponent<InputField>().text != "" && clientName == "Unknown")
        {
            clientName = GameObject.Find("TillNameInput").GetComponent<InputField>().text.ToString();
            if (cController.instance != null)
            {
                cController.instance.tillName = clientName.ToString();
            }
        }

        //Find the IP input
        string h;
        h = GameObject.Find("HostInput").GetComponent<InputField>().text.ToString();
        if (h != "")
        {
            hostIP = h;
        }
        //Find the Port input
        int p;
        int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out p);
        if(p != 0)
        {
            hostPort = p;
        }

        /*
         * ERROR: Missing clientName and hostIP Variables
         * DESCRIPTION: Variable defintion not working
         * STATUS: Resolved
         * SOLUTION: Changing Intital values in Unity Engine Window
         */

        //If the values are defaulted then don't continue
        if(hostPort == 0 || hostIP == "192.168.1.")
        {
            Debug.Log("Connection Failed 2");
            return;
        }

        //create the socket
        try
        {
            socket = new TcpClient(hostIP, hostPort);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
            connected = true;
            hostPanel.SetActive(false);
            StartCoroutine(SendingData());
        }
        catch (Exception e) //if there is an error (means the program doesn't crash)
        {
            Debug.Log("Socket error : " + e.Message);
            CreateErrorPopup(e.Message);
            return;
        }
        //If there is a category reciever
        if (catReciever != null)
            {
                StartCoroutine(catReciever.RequestCategories());
            }
        Debug.Log("Connected to server");
        //Save the data used to connect
        FindObjectOfType<SaveLoadController>().SaveData();
    }

    //runs every frame
    private void Update()
    {
        //checks that if connected to a server
        if (socketReady)
        {
            //checks it can recieve data
            if (stream.DataAvailable)
            {
                //checks for any incoming data
                string data = reader.ReadLine();
                if (data != null)
                {
                    OnIncomingData(data);
                }
            }
        }
    }

    //Called when data is recieved from the server
    private void OnIncomingData(string rawData)
    {
        //decrypts the data
        string data = encrypter.instance.Decrypt(rawData);
        //Debug.Log("Sever : " + data);

        //Checks if the server needs to respond
        if (!data.Contains("RECIEVED"))
        {
            toSend.AddFirst("&RECIEVED");
        }

        //Works out what the data is for and processes it
        if (data == "%NAME") //Server is requesting a name
        {
            
            toSend.AddLast(("&NAME |" + clientName));
            return;
        }else if (data.Contains("%STOCKLURT")) //Server is retuning stock data
        {
            string[] splitData = data.Split('|'); //Split up the data
            if(splitData[1] == "~END") //Check for the end element
            {
                stockLookup.instance.allItemsReturned = true;
                return;
            }
            else 
            { 
                Item itemToAdd = new(Convert.ToInt64(splitData[1]), splitData[2].ToString(), (float)Convert.ToDouble(splitData[3]), Convert.ToInt32(splitData[4]));
                stockLookup.returnedItems.Add(itemToAdd);           
                return;
            }
        }
        else if (data.Contains("CATEGORYRT")) //Server is returning category data
        {
            string[] splitData = data.Split('|'); //Splits up the data 
            if (splitData[1] == "~END") //Checks for the end element
            {
                catReciever.instance.allCategoriesRecieved = true;               
                return;
            }
            else
            { 
                ColorUtility.TryParseHtmlString(splitData[3], out Color returnedColour);
                Category catToAdd = new(Convert.ToInt32(splitData[1]), splitData[2], returnedColour, new Item[20]);
                catReciever.categoriesRecieved.Add(catToAdd);                
                return;
            }
        }
        else if (data.Contains("%CATEGORYITEMRT")) //Server is returning category item data
        {
            string[] splitData = data.Split('|'); //Splits up the data
            if (splitData[1] == "~END") //Checks for the end element
            {
                catReciever.instance.allCategoryItemsReceived = true;               
                return;
            }
            else
            {
                catReciever.categoryItemReturn.Add(new KeyValuePair<long, int>(Convert.ToInt64(splitData[1]), Convert.ToInt32(splitData[2])));               
                return;
            }
        }        
        else if (data.Contains("%CATEGORYITEMNAMERT")) //Server is returning category item name data
        {
            string[] splitData = data.Split('|'); //Splits up the data
            if (splitData[1] != "~END") //Checks for the end element
            {
                catReciever.itemReturn = new Item(Convert.ToInt64(splitData[1]), splitData[2], (float)Convert.ToDouble(splitData[3]), Convert.ToInt32(splitData[4]));  
                return;
            }
        }
        else if (data.Contains("%STAFFLOGINRT")) //Server is returning staff data
        {
            string[] splitData = data.Split('|'); //Splits up the data
            if (splitData[1] == "~END") //Checks for then end element
            {
                stafflogin.instance.allStaffMembersReturned = true;               
                return;
            }
            else
            {
                int idReturned = Convert.ToInt32(splitData[1]);
                string lastNameReturned = splitData[2].ToString();
                string firstNameReturned = splitData[3].ToString();
                DateTime dateOfBirthReturned = DateTime.Parse(splitData[4].ToString());
                DateTime startDateReturned = DateTime.Parse(splitData[5].ToString());
                DateTime endDateReturned = DateTime.Parse(splitData[6].ToString());
                int permissionLevelReturned = Convert.ToInt32(splitData[7]);
                stafflogin.instance.returnedStaffMembers.Add(new StaffMember(idReturned, lastNameReturned, firstNameReturned, dateOfBirthReturned, startDateReturned, endDateReturned, permissionLevelReturned));               
                return;
            }
        }
        else if (data.Contains("%AUTHSTAFFRT")) //Server is returning the authorising staff data
        {
            string[] splitData = data.Split('|');//Splits up the data

            int idReturned = Convert.ToInt32(splitData[1]);
            string lastNameReturned = splitData[2].ToString();
            string firstNameReturned = splitData[3].ToString();
            DateTime dateOfBirthReturned = DateTime.Parse(splitData[4].ToString());
            DateTime startDateReturned = DateTime.Parse(splitData[5].ToString());
            DateTime endDateReturned = DateTime.Parse(splitData[6].ToString());
            int permissionLevelReturned = Convert.ToInt32(splitData[7]);
            FindObjectOfType<PaymentController>().instance.returnedStaffMember = new StaffMember(idReturned, lastNameReturned, firstNameReturned, dateOfBirthReturned, startDateReturned, endDateReturned, permissionLevelReturned);            
            return;
        }
        else if (data.Contains("%DIRECTSTOCKRT")) //Server is reutning a individual stock item
        {
            string[] splitData = data.Split('|'); //Splits up the data
            if (splitData[1] == "ERROR") //Checks if an error has been returned
            {
                //Creates a popup
                CreateErrorPopup("Error: No or mulitple data entrires returned");
            }
            else
            {
                cController.AddItemToOrder(new Item(Convert.ToInt64(splitData[1]), splitData[2], (float)Convert.ToDouble(splitData[3]), Convert.ToInt32(splitData[4])));               
                return;
            }
        }
        else if (data.Contains("%RECIEVED") && toSend.Count > 0) //Server has recieved the data the client has sent
        {
            toSend.RemoveFirst();
            return;
        }
    }

    //Called when the client has data to send to the server
    public void Send(string rawData)
    {
        //If the socket isn't ready to be used then return
        if (!socketReady)
        {
            return;
        }
        //Encrypt the data
        string data = encrypter.instance.Encrypt(rawData);
        //Send the data
        writer.WriteLine(data);
        writer.Flush();
        //If the data is a recieved statement, remove it from the linked list
        if (rawData.Contains("RECIEVED"))
        {
            if (toSend.Count != 0)
            {
                toSend.RemoveFirst();
            }
        }
    }

    //Checks if client can send data to the server at the end of each frame
    IEnumerator SendingData()
    {
        while (true) //Run infintly
        {
            if (toSend.Count != 0) //If there is data to send
            {
                string toSendCurrent = toSend.First();
                Send(toSend.First()); //Send the data
                yield return new WaitUntil(() => toSend.FirstOrDefault() != toSendCurrent); //When the first item in the linked list is no-longer the item sent
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    //Used to close the connection to the server
    private void CloseScoket()
    {
        if (!socketReady)
        {
            return;
        }
        //closes all of the parts of the connection
        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    //when the application is exited
    private void OnApplicationQuit()
    {
        CloseScoket();
    }

    //when the gameObject is deactivated
    private void OnDisable()
    {
        CloseScoket();
    }

    //Creates an error pop-up with the specified message
    public void CreateErrorPopup(string errorMessage)
    {
        GameObject newPopup = Instantiate(errorPopupPrefab, errorPopupHolder.transform);
        newPopup.GetComponent<ErrorPopupController>().SetData(errorMessage);
    }
}
