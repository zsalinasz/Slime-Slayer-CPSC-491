using System.Collections.Generic;
using UnityEngine;

public static class ItemDataManager
{
    private static Dictionary<string, string> descriptions;

    public static void LoadDescriptions()
    {
        TextAsset file =
            Resources.Load<TextAsset>("Data/item_descriptions");

        ItemDescriptionCollection collection =
            JsonUtility.FromJson<ItemDescriptionCollection>(file.text);

        descriptions = new Dictionary<string, string>();

        foreach (ItemDescriptionEntry entry in collection.descriptions)
        {
            descriptions[entry.key] = entry.text;
        }
    }

    public static string GetDescription(string key)
    {
        if (descriptions == null)
            LoadDescriptions();

        if (descriptions.TryGetValue(key, out string description))
            return description;

        return "No description available.";
    }
}

[System.Serializable]
public class ItemDescriptionEntry
{
    public string key;
    public string text;
}

[System.Serializable]
public class ItemDescriptionCollection
{
    public ItemDescriptionEntry[] descriptions;
}
