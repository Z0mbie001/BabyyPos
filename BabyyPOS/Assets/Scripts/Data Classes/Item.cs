using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    int id = 0;
    string name = "Item";
    float price = 0.5f;
    int type = 0;

    public Item(int id, string name, float price, int type)
    {
        this.id = id;
        this.name = name;
        this.price = price;
        this.type = type;
    }
}
