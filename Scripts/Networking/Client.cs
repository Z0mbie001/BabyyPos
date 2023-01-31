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
    public LinkedList<string> toSend = new LinkedList<string>();

    [Header("GameObjects")]
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
        instance = this;
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
            StartCoroutine(sendingData());
            if (catReciever.instance != null)
            {
                StartCoroutine(catReciever.RequestCategories());
            }
            Debug.Log("Connected to server");
            FindObjectOfType<SaveLoadController>().instance.SaveData();
        }
        catch (Exception e) //if there is an error
        {
            Debug.Log("Socket error : " + e.Message);
        }
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

        //Works out what the data is for
        if (data == "%NAME")
        {
            //Server is requesting a name
            toSend.AddFirst("&RECIEVED");
            toSend.AddLast(("&NAME |" + clientName));
            return;
        }else if (data.Contains("%STOCKLURT"))
        {
            //Server is returning StockLookup data
            string[] splitData = data.Split('|');
            if(splitData[1] == "~END")
            {
                stockLookup.instance.allItemsReturned = true;
                toSend.AddFirst("&RECIEVED");
                return;
            }
            else
            {
                //Debug.Log(splitData[0] + " " + splitData[1] + " " + splitData[2] + " " + splitData[3] + " " + splitData[4]);
                Item itemToAdd = new Item(Convert.ToInt32(splitData[1]), splitData[2].ToString(), (float)Convert.ToDouble(splitData[3]), Convert.ToInt32(splitData[4]));
                stockLookup.returnedItems.Add(itemToAdd); //itemToAdd being returned as null
                toSend.AddFirst("&RECIEVED");
                //toSend.Enqueue("&RECIEVED");
                return;
            }
        }
        else if (data.Contains("CATEGORYRT"))
        {
            //Server is returning Category data
            string[] splitData = data.Split('|');   
            if (splitData[1] == "~END")
            {
                catReciever.instance.allCategoriesRecieved = true;
                toSend.AddFirst("&RECIEVED");
                return;
            }
            else
            {
                Color returnedColour;
                ColorUtility.TryParseHtmlString(splitData[3], out returnedColour);
                Category catToAdd = new Category(Convert.ToInt32(splitData[1]), splitData[2], returnedColour, new Item[20]);
                catReciever.categoriesRecieved.Add(catToAdd);
                toSend.AddFirst("&RECIEVED");
                return;
            }
        }
        else if (data.Contains("%CATEGORYITEMRT"))
        {
            //Server is returning category item data
            string[] splitData = data.Split('|');
            if (splitData[1] == "~END")
            {
                catReciever.instance.allCategoryItemsReceived = true;
                toSend.AddFirst("&RECIEVED");
                return;
            }
            else
            {
                catReciever.categoryItemReturn.Add(new KeyValuePair<int, int>(Convert.ToInt32(splitData[1]), Convert.ToInt32(splitData[2])));
                toSend.AddFirst("&RECIEVED");
                return;
            }
        }        
        else if (data.Contains("%CATEGORYITEMNAMERT"))
        {
            //Server is returning the category item name
            string[] splitData = data.Split('|');
            if (splitData[1] == "~END")
            {
                //catReciever.instance.allCategoryItemsReceived = true;
                toSend.AddFirst("&RECIEVED");
                return;
            }
            else
            {
                catReciever.itemReturn = new Item(Convert.ToInt32(splitData[1]), splitData[2], (float)Convert.ToDouble(splitData[3]), Convert.ToInt32(splitData[4]));
                toSend.AddFirst("&RECIEVED");
                return;
            }
        }
        else if (data.Contains("%STAFFLOGINRT"))
        {
            //Server is returning staff data
            string[] splitData = data.Split('|');
            if (splitData[1] == "~END")
            {
                stafflogin.instance.allStaffMembersReturned = true;
                toSend.AddFirst("&RECIEVED");
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
                toSend.AddFirst("&RECIEVED");
                return;
            }
        }
        else if (data.Contains("%AUTHSTAFFRT"))
        {
            //Server is returning data for a authorising staff member
            string[] splitData = data.Split('|');
            int idReturned = Convert.ToInt32(splitData[1]);
            string lastNameReturned = splitData[2].ToString();
            string firstNameReturned = splitData[3].ToString();
            DateTime dateOfBirthReturned = DateTime.Parse(splitData[4].ToString());
            DateTime startDateReturned = DateTime.Parse(splitData[5].ToString());
            DateTime endDateReturned = DateTime.Parse(splitData[6].ToString());
            int permissionLevelReturned = Convert.ToInt32(splitData[7]);
            FindObjectOfType<PaymentController>().instance.returnedStaffMember = new StaffMember(idReturned, lastNameReturned, firstNameReturned, dateOfBirthReturned, startDateReturned, endDateReturned, permissionLevelReturned);
            toSend.AddFirst("&RECIEVED");
            return;
        }
        else if (data.Contains("%RECIEVED") && toSend.Count > 0)
        {
            //Server confirmed that it has recieved the data
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
    IEnumerator sendingData()
    {
        while (true)
        {
            //Debug.Log("Client checking to send");
            if (toSend.Count != 0)
            {
                //Debug.Log(toSend.Peek());
                string toSendCurrent = toSend.First();
                Send(toSend.First());
                yield return new WaitUntil(() => toSend.FirstOrDefault() != toSendCurrent);
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
}
