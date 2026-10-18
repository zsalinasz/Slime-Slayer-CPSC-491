using UnityEngine;

public static class ItemIconLookup
{
    // Default: loads Assets/Resources/ItemIcons/<descriptionKey>.png
    public static Sprite GetIcon(Item item)
    {
        return Resources.Load<Sprite>("ItemIcons/" + item.descriptionKey);
    }
}