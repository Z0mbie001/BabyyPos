using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;

public class Server : MonoBehaviour
{
    //initalising variables
    public int port = 2801;
    private TcpListener server;
    private bool serverStarted;

    private List<ServerClient> clients;
    private List<ServerClient> disconectList;

    public Server instance;

    //Run before first frame, starts the server
    private void Start()
    {
        instance = this;
        clients = new List<ServerClient>();
        disconectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
            Debug.Log("Sever has beens started on port " + port.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    //run every frame, checks for new requests and if the client is still connected
    private void Update()
    {
        if (!serverStarted)
        {
            return;
        }
        foreach (ServerClient c in clients)
        {
            //is the client still connected?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconectList.Add(c);
                continue;
            }
            //check for message from the client
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                    {
                        OnIncomingData(c, data);
                    }
                }
            }
        }

        for (int i = 0; i < disconectList.Count - 1; i++)
        {
            //Broadcast(disconectList[i].clientName + " has dissconeted", clients);
            Debug.Log("Client disconnected : " + disconectList[i].clientName);
            clients.Remove(disconectList[i]);
            disconectList.RemoveAt(i);
        }
    }

    //called when new data is recieved
    private void OnIncomingData(ServerClient c, string data)
    {
        if (data.Contains("&NAME"))
        {
            c.clientName = data.Split('|')[1];
            Debug.Log("Name Returned from " + c.clientName);
            //Broadcast(c.clientName + " Has Connected!", clients);
        }else if (data.Contains("&STOCKLUDBR"))
        {
            string[] splitData = data.Split('|');
            FindObjectOfType<ServerController>().instance.StockLookupRequest(c, System.Convert.ToInt32(splitData[1]), splitData[2]);
            Debug.Log("Stock Lookup request from " + c.clientName + " has been received with the parameters : " + splitData[1] + " and " + splitData[2]);
        }
        else
        {
            Debug.Log(c.clientName + " has sent the following message: " + data);
            //Broadcast(c.clientName + ": " + data, clients);
        }
    }

    //starts the server listening for new requests
    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    //checks if a client is connected still
    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    //accepts the clients' connection requests
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        //request a name from the client that just connected
        Send("%NAME", clients[clients.Count - 1]);
        Debug.Log("Requesting Client Name");
    }

    //used to send data to the clients
    public void Send(string data, ServerClient c)
    {
        try
        {
            StreamWriter writer = new StreamWriter(c.tcp.GetStream());
            writer.WriteLine(data);
            writer.Flush();
        }
        catch (Exception e) //if there is an error
        {
            Debug.Log("Write error : " + e.Message + "to client " + c.clientName);
        }

    }
}

//A class to store client details in
public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    //initilising funciton
    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Unknown";
        tcp = clientSocket;
    }
}
