
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootBoxEntry
{
    public ItemData item;
    [Min(1)] public int quantity = 1;
}

public class LootBox : MonoBehaviour
{
    [Header("Loot Contents")]
    [SerializeField]
    private List<LootBoxEntry> items =
        new List<LootBoxEntry>();

    public event Action OnLootChanged;

    public IReadOnlyList<LootBoxEntry> Items => items;

    public bool TakeItem(int index)
    {
        if (index < 0 || index >= items.Count)
            return false;

        LootBoxEntry entry = items[index];

        if (entry == null || entry.item == null ||
            entry.quantity <= 0)
        {
            Debug.LogWarning(
                "LootBox: Invalid loot entry."
            );
            return false;
        }

        PlayerDataManager manager =
            PlayerDataManager.Instance;

        if (manager == null)
        {
            Debug.LogWarning(
                "LootBox: PlayerDataManager not found."
            );
            return false;
        }

        // Only remove loot if the inventory accepts it.
        if (!manager.AddItem(entry.item, entry.quantity))
        {
            Debug.LogWarning(
                "LootBox: Inventory rejected the item."
            );
            return false;
        }

        items.RemoveAt(index);
        OnLootChanged?.Invoke();

        return true;
    }
}
