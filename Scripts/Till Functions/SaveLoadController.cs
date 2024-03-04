using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    public SaveLoadController instance;

    [Header("JSON Save Path")]
    private string jsonSavePath;
    /*
     * ERROR: UnityException: get_persistentDataPath is not allowed to be called from a MonoBehaviour constructor
     * DESCRIPTION: Cannot find the persistant path when called outside of a class method
     * STATUS: Resolved
     * SOLUTION: Move variable definition to Start()
     */

    [Header("Booleans")]
    public bool loadCompleted;

    [Header("References")]
    private Client client;
    private ClientController clientController;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        client = FindObjectOfType<Client>();
        if (FindObjectOfType<ClientController>() != null)
        {
            clientController = FindObjectOfType<ClientController>().instance;
        }
        jsonSavePath = Application.persistentDataPath.ToString() + "/ClientSaveData.json";
        LoadData();
    }

    //Fetches the data needed to save
    public SaveData GetDataToSave()
    {
        SaveData data = new SaveData
        {
            tillName = client.clientName,
            hostIP = client.hostIP,
            hostPort = client.hostPort
        };
        if (clientController != null)
        {
            data.transactionNumber = clientController.transactionNumber;
        }
        else
        {
            data.transactionNumber = 888;
        }
        return data;
    }

    //Saves the data to a file
    public void SaveData()
    {
        SaveData dataToSave = GetDataToSave();
        string jsonData = JsonUtility.ToJson(dataToSave, true);
        File.WriteAllText(jsonSavePath, jsonData);
        Debug.Log("Saved Data");
    }

    //Loads the data from a file
    public void LoadData()
    {
        loadCompleted = false;
        try
        {
            SaveData loadedData = JsonUtility.FromJson<SaveData>(File.ReadAllText(jsonSavePath));
            client.clientName = loadedData.tillName;
            client.hostIP = loadedData.hostIP;
            client.hostPort = loadedData.hostPort;
            if (clientController != null)
            {
                clientController.tillName = loadedData.tillName;
                clientController.transactionNumber = loadedData.transactionNumber;
            }
            loadCompleted = true;
            Debug.Log("Loaded Data from file");
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveLoad Error : " + e.Message);
        }
        if (!client.connected)
        {
            client.ConnectToServer();
        }
    }

    /*
     * ERROR: Missing path for file read
     * DESCRIPTION: Load data statment missling a file path
     * STATUS: Resolved
     * SOLUTION: Re-order start procedure run order
     */

    /*
     * ERROR: Client attempting to connect to early
     * DESCRIPTION: Client Calling connectToServer before data loaded
     * STATUS:
     * SOLUTION: Call Connect to server at the end of the load data procedure
     */

    //Runs when the application closes
    private void OnApplicationQuit()
    {
        SaveData();
    }
}
