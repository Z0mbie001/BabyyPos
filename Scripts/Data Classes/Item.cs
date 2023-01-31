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
    public Item(int idToSet, string nameToSet, float priceToSet, int typeToSet)
    {
        this.id = idToSet;
        this.name = nameToSet;
        this.price = priceToSet;
        this.type = typeToSet;
    }
}
