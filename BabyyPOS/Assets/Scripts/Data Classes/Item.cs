using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int id = 0;
    public string name = "Item";
    public float price = 0.5f;
    public int type = 0;

    //initalising function
    public Item(int id, string name, float price, int type)
    {
        this.id = id;
        this.name = name;
        this.price = price;
        this.type = type;
    }
}
