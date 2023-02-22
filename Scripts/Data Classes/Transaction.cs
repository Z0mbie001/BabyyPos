using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Transaction
{
    public long transactionID;
    public DateTime transactionDateTime;
    public float transactionAmount;
    public int paymentType;
    public int staffID;
    public List<(Item, int)> transactionItems = new List<(Item, int)>(); //item and quantity (place price in item)

    //Initilsing procedure
    public Transaction(long transIDToAdd, DateTime transDateTimeToAdd, float transAmountToAdd, int payTypeToAdd, int staffIDToAdd, List<(Item, int)> itemsToAdd)
    {
        transactionID = transIDToAdd;
        transactionDateTime = transDateTimeToAdd;
        transactionAmount = transAmountToAdd;
        paymentType = payTypeToAdd;
        staffID = staffIDToAdd;
        transactionItems = itemsToAdd;
    }
}
