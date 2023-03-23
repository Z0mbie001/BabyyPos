using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Category 
{
    //Attribute definition
    public int categoryID;
    public string categoryName;
    public Color categoryColour;
    public Item[] itemsInCategory = new Item[20];

    //Initlising procedure
    public Category(int categoryID, string categoryName, Color categoryColour, Item[] itemsInCategory)
    {
        this.categoryID = categoryID;
        this.categoryName = categoryName;
        this.categoryColour = categoryColour;
        this.itemsInCategory = itemsInCategory;
    }
}
