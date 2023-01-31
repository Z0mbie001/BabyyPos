using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.Rendering;
using System;

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
        //Makes sure that all the tables required exist
        CreateStockTable();
        CreateCategoryTable();
        CreateCategoryItemTable();
        CreateStaffTable();
        CreateTransactionTable();
        CreateTransactionItemTable();
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
        Debug.Log("Successfully connected to the database");
        return dbConnection;
    }

    //Create the stock table
    void CreateStockTable()
    {
        //create a command
        IDbCommand dbCommand;
        dbCommand = dbcon.CreateCommand();
        //create the SQL command and execute it
        string q_createTable = "CREATE TABLE IF NOT EXISTS stock_table (id INTEGER PRIMARY KEY, name VARCHAR(255), price FLOAT, type INTEGER);";
        dbCommand.CommandText = q_createTable;
        dbCommand.ExecuteReader();
    }

    //Reads values in the stock table
    public List<Item> ReadValuesInStockTable(int id, string itemName)
    {
        Debug.Log("Requesting Read Values From Stock Table");
        //creates a temp list to store values and a temp string
        List<Item> items = new List<Item>();
        string tempIdString;
        //create a command and reader
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        //check to see if id == 0
        if(id == 0)
        {
            tempIdString = "";
        }
        else
        {
            tempIdString = id.ToString();
        }
        //create the SQL command and execute it
        string q_readTable = "SELECT * FROM stock_table WHERE id LIKE '%" + tempIdString + "%' AND name LIKE '%" + itemName + "%' ORDER BY id ASC";
        cmnd_read.CommandText = q_readTable;
        reader = cmnd_read.ExecuteReader();
        Debug.Log(q_readTable);
        //while the reader is recieveing data from the database
        while (reader.Read())
        {
            //format the data 
            int idRead = Convert.ToInt32(reader[0]);
            string nameRead = Convert.ToString(reader[1]);
            float priceRead = (float)Convert.ToDouble(reader[2]);
            int typeRead = Convert.ToInt32(reader[3]);
            items.Add(new Item(idRead, nameRead, priceRead, typeRead));
        }
        //return all the items
        return items;
    }

    //Inserts values into the stock table
    public void InsertValueIntoStockTable(int id, string itemName, float price, int type)
    {
        IDbCommand cmnd_insert = dbcon.CreateCommand();
        string q_insertTable = "INSERT OR REPLACE INTO stock_table (id, name, price, type) VALUES (" + id + ", '" + itemName + "', " + price + ", " + type + ");";
        cmnd_insert.CommandText = q_insertTable;
        cmnd_insert.ExecuteNonQuery();
    }

    //Deletes a value in the stock table
    public void DeleteValueInStockTable(int id)
    {
        IDbCommand cmnd_delete = dbcon.CreateCommand();
        string q_deleteItem = "DELETE FROM stock_table WHERE id LIKE " + id + ";";
        cmnd_delete.CommandText = q_deleteItem;
        cmnd_delete.ExecuteNonQuery();
    }

    //create the catagory table
    void CreateCategoryTable()
    {
        IDbCommand dbCommand;
        dbCommand= dbcon.CreateCommand();
        string q_createTable = "CREATE TABLE IF NOT EXISTS category_table (id INTEGER PRIMARY KEY, name VARCHAR(255), color VARCHAR(255));";
        dbCommand.CommandText = q_createTable;
        dbCommand.ExecuteReader();
    }

    //Reads values from the Category Table
    public List<Category> ReadValuesInCategoryTable(int id)
    {
        List<Category> categories = new List<Category>();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string q_readTable = "SELECT * FROM category_table WHERE id LIKE '%" + id + "' ORDER BY id ASC;";
        cmnd_read.CommandText = q_readTable;
        reader = cmnd_read.ExecuteReader();
        while(reader.Read())
        {
            int catIDRead = Convert.ToInt32(reader[0]);
            string catNameRead = Convert.ToString(reader[1]);
            UnityEngine.Color catColourRead;
            ColorUtility.TryParseHtmlString(reader[2].ToString(), out catColourRead);
            categories.Add(new Category(catIDRead, catNameRead, catColourRead, new Item[20]));
        }
        return categories;
    }

    //Inserts values into the category table
    public void InsertValuesIntoCategoryTable(int id, string categoryName, UnityEngine.Color categoryColour)
    {
        IDbCommand cmnd_insert = dbcon.CreateCommand();
        string q_insertTable = "INSERT OR REPLACE INTO category_table (id, name, colour) VALUES (" + id + ", '" + categoryName + "', '" + ColorUtility.ToHtmlStringRGB(categoryColour).ToString() + "');";
        cmnd_insert.CommandText = q_insertTable;
        cmnd_insert.ExecuteNonQuery();
    }

    //Creates the cateogry item Table
    void CreateCategoryItemTable()
    {
        IDbCommand dbCommand = dbcon.CreateCommand();
        string q_createTable = "CREATE TABLE IF NOT EXISTS categoryItem_table (categoryID INTEGER, itemID INTEGER, itemPos INTEGER, PRIMARY KEY(categoryID, itemID));";
        dbCommand.CommandText = q_createTable;
        dbCommand.ExecuteReader();
    }

    //Reads values from the category item table
    public List<KeyValuePair<int, int>> ReadValuesInCategoryItemTable(int categoryID)
    {
        List<KeyValuePair<int, int>> items = new List<KeyValuePair<int, int>>();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string q_readTable = "SELECT itemID, itemPos FROM categoryItem_table WHERE categoryID LIKE '" + categoryID + "';";
        cmnd_read.CommandText = q_readTable;
        reader = cmnd_read.ExecuteReader();
        while(reader.Read())
        {
            int itemIdRead = Convert.ToInt32(reader[0]);
            int posRead = Convert.ToInt32(reader[1]);
            items.Add(new KeyValuePair<int, int>(itemIdRead, posRead));
        }
        return items;
    }

    //Inserts values into the category item table
    public void InsertValuesIntoCategoryItemsTable(int categoryID, int itemID, int itemPos)
    {
        IDbCommand cmnd_insert = dbcon.CreateCommand();
        string q_insertTable = "INSERT OR REPLACE INTO categoryItem_table (categoryId, itemID, itemPos) VALUES (" + categoryID + ", " + itemID + ", " + itemPos + ");";
        cmnd_insert.CommandText = q_insertTable;
        cmnd_insert.ExecuteNonQuery();
    }

    //creates the staff table
    void CreateStaffTable()
    {
        IDbCommand dbCommand = dbcon.CreateCommand();
        string q_createTable = "CREATE TABLE IF NOT EXISTS staff_table (staffID INTEGER PRIMARY KEY, lastName VARCHAR(255), firstName VARCHAR(255), dateOfBirth VARCHAR(255), startDate VARCHAR(255), endDate VARCHAR(255), permissionsLv INTEGER);";
        dbCommand.CommandText = q_createTable;
        dbCommand.ExecuteReader(); 
    }

    //Reads values from the staff table
    public List<StaffMember> ReadStaffMembersInTable(int staffID, string staffLastName, string staffFirstName)
    {
        List<StaffMember> data = new List<StaffMember>();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string q_readTable = "SELECT * FROM staff_table WHERE staffID LIKE '%" + staffID.ToString() + "%' AND lastName LIKE '%" + staffLastName + "%' AND firstName LIKE '%" + staffFirstName + "%';";
        cmnd_read.CommandText = q_readTable;
        reader = cmnd_read.ExecuteReader();
        while(reader.Read())
        {
            int staffIDRead = Convert.ToInt32(reader[0]);
            string lastNameRead = reader[1].ToString();
            string firstNameRead = reader[2].ToString();
            DateTime dateOfBirthRead = DateTime.Parse(reader[3].ToString());
            DateTime startDateRead = DateTime.Parse(reader[4].ToString());
            DateTime endDateRead = DateTime.Parse(reader[5].ToString());
            int permissionsLevelRead = Convert.ToInt32(reader[6]);
            data.Add(new StaffMember(staffIDRead, lastNameRead, firstNameRead, dateOfBirthRead, startDateRead, endDateRead, permissionsLevelRead));
        }
        return data;
    }

    //Inserts values into the staff table
    public void InsertValuesIntoStaffTable(StaffMember staffMemberToAdd)
    {
        IDbCommand cmnd_insert = dbcon.CreateCommand();
        string q_insertTable = "INSERT OR REPLACE INTO staff_table (staffID, lastName, firstName, dateOfBirth, startDate, endDate, permissionsLv) VALUES (" + staffMemberToAdd.staffID.ToString() + ", '" + staffMemberToAdd.lastName + "', '" + staffMemberToAdd.firstName + "', '" + staffMemberToAdd.dateOfBirth.ToString("s") + "', '" + staffMemberToAdd.startDate.ToString("s") + "', '" + staffMemberToAdd.endDate.ToString("s") + "', " + staffMemberToAdd.permissionLevel.ToString() + ");";
        cmnd_insert.CommandText = q_insertTable;
        cmnd_insert.ExecuteNonQuery();
    }

    //Creates the transaction table
    void CreateTransactionTable()
    {
        IDbCommand dbCommand = dbcon.CreateCommand();
        string q_createTable = "CREATE TABLE IF NOT EXISTS transaction_table (transactionID INTEGER PRIMARY KEY, transactionDateTime VARCHAR(255), transactionTotal FLOAT, paymentType INTEGER, staffID INTEGER);";
        dbCommand.CommandText = q_createTable;
        dbCommand.ExecuteReader();
    }

    //Reads values from the transaction table
    public List<Transaction> ReadValuesFromTransactionTable(int transID, DateTime transDateTime, int staffIDToFind, float transTotal)
    {
        List<Transaction> data = new List<Transaction>();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string q_readTable = "SELECT * FROM transaction_table WHERE transID Like '%" + transID.ToString() + "%' AND transactionDateTime LIKE '%" + transDateTime.ToString("s") + "%' OR transactionTotal = " + transTotal + " AND staffID LIKE '%" + staffIDToFind.ToString() + "%';";
        cmnd_read.CommandText = q_readTable;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            int transIDRead = Convert.ToInt32(reader[0]);
            DateTime transDateTimeRead = DateTime.Parse(reader[1].ToString());
            float transTotalRead = (float)Convert.ToDouble(reader[2]);
            int payTypeRead = Convert.ToInt32(reader[3]);
            int staffIdRead = Convert.ToInt32(reader[4]);
            data.Add(new Transaction(transIDRead, transDateTimeRead, transTotalRead, payTypeRead, staffIdRead, new List<(Item, int)>()));
        }
        return data;
    }

    //Inserts values into the transaction table
    public void InsertValuesIntoTransTable(int transID, DateTime transDateTime, float transTotal, int payType, int staffID)
    {
        IDbCommand cmnd_insert = dbcon.CreateCommand();
        string q_insertTable = "INSERT OR REPLACE INTO transaction_table (transactionID, transactionDateTime, transactionTotal, paymentType, staffID) VALUES (" + transID.ToString() + ", '" + transDateTime.ToString("s") + "', '" + transTotal.ToString() + "', " + payType.ToString() + ", " + staffID + ");";
        cmnd_insert.CommandText = q_insertTable;
        cmnd_insert.ExecuteNonQuery();
    }

    //creeates the transaction item table
    void CreateTransactionItemTable()
    {
        IDbCommand dbCommand = dbcon.CreateCommand();
        string q_createTable = "CREATE TABLE IF NOT EXISTS transactionItem_table (transactionID INTEGER, itemID INTEGER, quantity INTEGER, price FLOAT, PRIMARY KEY(transactionID, itemID));";
        dbCommand.CommandText = q_createTable;
        dbCommand.ExecuteReader();
    }

    //Reads values from teh transaction item table
    public List<(Item, int)> ReadValuesFromTransactionItemTable(int transID)
    {
        List<(Item, int)> data = new List<(Item, int)> ();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string q_readTable = "SELECT * FROM transactionItem_table WHERE transID LIKE '%" + transID.ToString() + "%';";
        cmnd_read.CommandText = q_readTable;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            Item item = new Item(Convert.ToInt32(reader[1]), "", Convert.ToInt32(reader[3]), 0);
            int quantityRead = Convert.ToInt32(reader[2]);
            data.Add((item, quantityRead));
        }
        return data;
    }

    //Inserts values into the transaction item table
    public void InsertValuesIntoTransItemTable(int transID, int itemID, int quantity, float price)
    {
        IDbCommand cmnd_insert = dbcon.CreateCommand();
        string q_insertTable = "INSERT OR REPLACE INTO transactionItem_table (transactionID, itemID, quantity, price) VALUES (" + transID.ToString() + ", " + itemID.ToString() + ", " + quantity.ToString() + ", " + price.ToString() + ");";
        cmnd_insert.CommandText = q_insertTable;
        cmnd_insert.ExecuteNonQuery();
    }
}
