using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        clientController = FindObjectOfType<ClientController>();
        jsonSavePath = Application.persistentDataPath.ToString() + "/ClientSaveData.json";
        LoadData();
    }

    //Fetches the data needed to save
    public SaveData GetDataToSave()
    {
        SaveData data = new SaveData();
        data.tillName = client.instance.clientName;
        data.hostIP = client.instance.hostIP;
        data.hostPort = client.instance.hostPort;
        if(clientController != null)
        {
            data.transactionNumber = clientController.instance.transactionNumber;
        }
        else
        {
            data.transactionNumber = -1;
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
            client.instance.clientName = loadedData.tillName;
            client.instance.hostIP = loadedData.hostIP;
            client.instance.hostPort = loadedData.hostPort;
            if (clientController.instance != null)
            {
                clientController.instance.tillName = loadedData.tillName;
                clientController.instance.transactionNumber = loadedData.transactionNumber;
            }
            loadCompleted = true;
            Debug.Log("Loaded Data from file");
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveLoad Error : " + e.Message);
        }
        if (!client.instance.connected)
        {
            client.instance.ConnectToServer();
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

    //Runs when the object is disabled
    private void OnDisable()
    {
        SaveData();
    }
}
