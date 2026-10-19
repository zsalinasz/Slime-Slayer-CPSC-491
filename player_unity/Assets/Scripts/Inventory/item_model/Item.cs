using System;
using UnityEngine;

[Serializable]
public class Item
{
    public string itemName;
    public string descriptionKey;

    public Item(string itemName, string descriptionKey)
    {
        this.itemName = itemName;
        this.descriptionKey = descriptionKey;
    }
}