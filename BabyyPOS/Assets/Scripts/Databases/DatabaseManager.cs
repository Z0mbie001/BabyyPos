using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    //makes this script an instance
    public DatabaseManager instance;
    //initilises the database connenction variable
    IDbConnection dbcon;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //create the database conneciton
        dbcon = OpenConenctionToDB();
        createStockTable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Creates the database conneciton
    IDbConnection OpenConenctionToDB()
    {
        //formats the database's location 
        string connection = "URI=file:" + Application.persistentDataPath + "/Babyy_Database.db";
        Debug.Log("Database Location : " + Application.persistentDataPath + "/Babyy_Database.db");
        //Opens the databse conneciton
        IDbConnection dbConnection = new SqliteConnection(connection);
        dbConnection.Open();
        //returns the new database connection
        return dbConnection;
    }

    //create the stock table
    void createStockTable()
    {
        //create a command
        IDbCommand dbCommand;
        dbCommand = dbcon.CreateCommand();
        //create the SQL command and execute it
        string q_createTable = "CREATE TABLE IF NOT EXISTS stock_table (id INTEGER PRIMARY KEY, name VARCHAR(255), price FLOAT, type INTEGER);";
        dbCommand.CommandText = q_createTable;
        dbCommand.ExecuteReader();
    }

    public List<Item> ReadValuesInStockTable(int id, string itemName)
    {
        //creates a temp list to store values
        List<Item> items = new List<Item>();
        //create a command and reader
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        //create the SQL command and execute it
        string q_readTable = "SELECT * FROM stock_table WHERE id LIKE '%" + id + "%' AND name LIKE '%" + itemName + "%'";
        cmnd_read.CommandText = q_readTable;
        reader = cmnd_read.ExecuteReader();
        //while the reader is recieveing data from the database
        while (reader.Read())
        {
            //format the data 
            int idRead = System.Convert.ToInt32(reader[0]);
            string nameRead = System.Convert.ToString(reader[1]);
            float priceRead = (float)System.Convert.ToDouble(reader[2]);
            int typeRead = System.Convert.ToInt32(reader[3]);
            items.Add(new Item(idRead, nameRead, priceRead, typeRead));
        }
        //return all the items
        return items;
    }

    public void InsertValueIntoStockTable(int id, string itemName, float price, int type)
    {
        IDbCommand cmnd_insert = dbcon.CreateCommand();
        string q_insertTable = "INSERT OR REPLACE INTO stock_table (id, name, price, type) VALUES (" + id + ", '" + itemName + "', " + price + ", " + type + ");";
        cmnd_insert.CommandText = q_insertTable;
        cmnd_insert.ExecuteNonQuery();
    }
}
