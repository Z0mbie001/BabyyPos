using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;
using System;

public class Client : MonoBehaviour
{
    public Client instance;
    public string clientName = "Unknown";
    
    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    //A proceudre that runs in the first frame
    private void Start()
    {
        instance = this;
    }

    //Allows the client to connect to the server
    public void ConnectToServer()
    {
        //if already connected, ignore this function
        if (socketReady)
        {
            return;
        }
        //Add the client Username
        if (GameObject.Find("TillNameInput").GetComponent<InputField>().text != "")
        {
            clientName = GameObject.Find("TillNameInput").GetComponent<InputField>().text;
        }

        //overide default host/port values if needed
        string host = "192.168.1.";
        int port = 2801;

        string h;
        int p;
        h = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (h != "")
        {
            host = h;
        }
        int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out p);
        if (p != 0)
        {
            port = p;
        }

        //create the socket
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;

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
    private void OnIncomingData(string data)
    {
        if (data == "%NAME")
        {
            Send("&NAME |" + clientName);
            return;
        }else if (data.Contains("%STOCKLURT"))
        {
            string[] splitData = data.Split('|');
            if(splitData[1] == "~END")
            {
                FindObjectOfType<StockLookup>().instance.allItemsReturned = true;
            }
            else
            {
                FindObjectOfType<StockLookup>().instance.returnedItems.Add(new Item(System.Convert.ToInt32(splitData[1]), splitData[2], (float)System.Convert.ToDouble(splitData[3]), System.Convert.ToInt32(splitData[4])));
            }
        }
        Debug.Log("Sever: " + data);
        //GameObject go = Instantiate(messagePrefab, chatContainer.transform) as GameObject;
        //go.GetComponentInChildren<Text>().text = data;
    }

    //Called when the client has data to send to the server
    public void Send(string data)
    {
        if (!socketReady)
        {
            return;
        }

        writer.WriteLine(data);
        writer.Flush();
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
