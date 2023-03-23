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
    readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!'£$%^&*()_+-=¬`|,<.>/?;:@[]{}~# ".ToCharArray();
    


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
        for(int i = 0; i < array.Length; i++) //Iterate through the length of the array
        {
            if(charToFind == array[i]) //If the current character is the one we want to find, return it
            {
                return i;
            }
        }
        return -1; //If the character isn't int the array, return empty
    }

    //Encrypts the data and returns the cypher text
    public string Encrypt(string plainText)
    { 
        string cypherText = "";
        for (int i = 0; i < plainText.Length; i++) //Iterate through the plainText
        {
            int keyCharNo = FindPosInArray(key[i % key.Count()], alphabet); //Find where we are in the key
            int plainTextCharNo = FindPosInArray((char)plainText[i], alphabet); //Find where the character is in the alphabet
            int cypherNumber = keyCharNo + plainTextCharNo; 
            cypherText += alphabet[cypherNumber % alphabet.Length]; //Add the new character to the array
        }
        return cypherText;
    }

    //Decrypts cypher text into plain text
    public string Decrypt(string cypherText)
    {
        string plainText = "";
        for(int i = 0; i < cypherText.Length; i++) //Iterate through the cypherText
        {
            int keyCharNo = FindPosInArray(key[i % key.Count()], alphabet); //Find where we are in the key
            int cypherTextCharNo = FindPosInArray((char)cypherText[i], alphabet); //Find where the character is in the alphabet
            int plainNumber = cypherTextCharNo - keyCharNo; 
            while (plainNumber < 0)
            {
                plainNumber += alphabet.Length;
            }
            plainText += alphabet[plainNumber % alphabet.Length]; //Add the decrypted character to the array
        }
        return plainText;
    }
}
