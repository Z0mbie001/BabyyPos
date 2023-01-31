using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Encryption : MonoBehaviour
{
    public Encryption instance;

    [Header("Encryption Data")]
    DateTime currentDate = DateTime.Today;
    string key = "";
    char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!'£$%^&*()_+-=¬`|,<.>/?;:@[]{}~# ".ToCharArray();
    


    //Start is called before the first frame
    void Start()
    {
        SetKey();
        instance = this;
    }

    //Update is called once per frame
    void Update()
    {
        //If the day has changed, update the key
        if(currentDate.Day != DateTime.Now.Day)
        {
            currentDate = DateTime.Now;
            SetKey();
        }
    }

    //Sets the ket to be used
    void SetKey()
    {
        key = currentDate.Date.ToString() + currentDate.DayOfYear;
        key = Encrypt(key);
        Debug.Log("Key: "+ key);
    }

    //Finds the position of an item in the array
    int FindPosInArray(char charToFind, char[] array)
    {
        for(int i = 0; i < array.Length; i++)
        {
            if(charToFind == array[i])
            {
                return i;
            }
        }
        return -1;
    }

    //Encrypts the data and returns the cypher text
    public string Encrypt(string plainText)
    { 
        string cypherText = "";
        for (int i = 0; i < plainText.Length; i++)
        {
            int keyCharNo = FindPosInArray(key[i % key.Count()], alphabet);
            int plainTextCharNo = FindPosInArray((char)plainText[i], alphabet);
            int cypherNumber = keyCharNo + plainTextCharNo;
            cypherText += alphabet[cypherNumber % alphabet.Length];
        }
        return cypherText;
    }

    //Decrypts cypher text into plain text
    public string Decrypt(string cypherText)
    {
        string plainText = "";
        for(int i = 0; i < cypherText.Length; i++)
        {
            int keyCharNo = FindPosInArray(key[i % key.Count()], alphabet);
            int cypherTextCharNo = FindPosInArray((char)cypherText[i], alphabet);
            int plainNumber = cypherTextCharNo - keyCharNo;
            while (plainNumber < 0)
            {
                plainNumber += alphabet.Length;
            }
            plainText += alphabet[plainNumber % alphabet.Length];
        }
        return plainText;
    }
}
