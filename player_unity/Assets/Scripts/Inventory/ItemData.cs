
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Item Information")]
    public string id;
    public string displayName;
    public Sprite icon;

    [TextArea]
    public string description;

    [Header("Item Classification")]
    public ItemType itemType = ItemType.Material;

    [Header("Inventory Settings")]
    [Min(1)]
    public int maxStack = 99;

    [Header("Economy")]
    [Min(0)]
    public int price = 0;
}
