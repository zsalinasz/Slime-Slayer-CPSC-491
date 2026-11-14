
using System;
using UnityEngine;

[Serializable]
public class Item
{
    // New item definition
    public ItemData data;

    // Quantity owned
    [Min(0)]
    public int quantity = 1;

    // Legacy fields for compatibility
    public string itemName;
    public string descriptionKey;

    // New constructor
    public Item(ItemData itemData, int amount = 1)
    {
        data = itemData;
        quantity = Math.Max(1, amount);

        itemName = itemData != null
            ? itemData.displayName
            : "";

        descriptionKey = itemData != null
            ? itemData.id
            : "";
    }

    // Existing constructor
    public Item(string name, string key)
    {
        itemName = name;
        descriptionKey = key;
        quantity = 1;
    }
}
