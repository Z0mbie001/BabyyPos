using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SaveData
{
    public string tillName;
    public string hostIP;
    public int hostPort;
    public int transactionNumber;

    //Initilising procedure
    public SaveData()
    {
        tillName = "Unknonw";
        hostIP = "192.168.1.";
        hostPort = 1803;
        transactionNumber = 0;
    }
}
