using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;


public class Server : MonoBehaviour
{
    public int port = 1803;
    private TcpListener server;
    private bool serverStarted;
    public Server instance;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            server = new TcpListener(port);
            server.Start();

            serverStarted = true;
            Debug.Log("Server has been stated on port " + port.ToString());
        }catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class ServerClient
    {
        public TcpClient tcp;
        public string tillName;

        public ServerClient(TcpClient clientSocket)
        {
            tillName = "Unknown";
            tcp = clientSocket;
        }
    }
}
