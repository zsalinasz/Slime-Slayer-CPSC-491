
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public PlayerData Data { get; private set; }

    private bool data_initialized = false;

    // CHANGE 1: Add these fields here
    [Header("Starting Inventory")]
    [SerializeField] private ItemData startingPotion;
    [SerializeField] private ItemData startingSword;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Data = new PlayerData();
        data_initialized = true;
    }

    void Start()
    {
        Debug.Log("Starting up the PlayerDataManager");

        Debug.Log(@$"Name: {Data.playerName}
        Level: {Data.level}
        Experience: {Data.experience}
        Current Health: {Data.currentHealth}
        Max Health: {Data.maxHealth}
        Current Stamina: {Data.currentStamina}
        Max Stamina: {Data.maxStamina}
        Strength: {Data.strength}
        Perception: {Data.perception}
        Agility: {Data.agility}
        Endurance: {Data.endurance}
        Currency: {Data.currency}");

        // CHANGE 2: Replace the old sample items
        // with ItemData-based items

        if (startingPotion != null)
        {
            Data.inventoryItemIds.Add(
                new Item(startingPotion, 3)
            );
        }

        if (startingSword != null)
        {
            Data.inventoryItemIds.Add(
                new Item(startingSword, 1)
            );
        }
    }
    // Check whether the player owns enough of an item
    public bool HasItem(ItemData itemData, int amount = 1)
    {
        if (Data == null || itemData == null || amount <= 0)
            return false;

        int total = 0;

        foreach (Item item in Data.inventoryItemIds)
        {
            if (item == null || item.data != itemData)
                continue;

            if (item.quantity < 0)
                return false;

            total += item.quantity;

            if (total >= amount)
                return true;
        }

        return false;
    }

    // Add items only if the request is valid
    public bool AddItem(ItemData itemData, int amount)
    {
        if (Data == null || itemData == null ||
            amount <= 0 || itemData.maxStack < 1)
        {
            Debug.LogWarning("Inventory: Invalid AddItem request.");
            return false;
        }

        Item existing = Data.inventoryItemIds.Find(
            item => item != null && item.data == itemData
        );

        if (existing != null)
        {
            if (existing.quantity < 0 ||
                existing.quantity > itemData.maxStack ||
                amount > itemData.maxStack - existing.quantity)
            {
                Debug.LogWarning(
                    "Inventory: Addition exceeds stack limit."
                );
                return false;
            }

            existing.quantity += amount;
        }
        else
        {
            if (amount > itemData.maxStack)
            {
                Debug.LogWarning(
                    "Inventory: Addition exceeds stack limit."
                );
                return false;
            }

            Data.inventoryItemIds.Add(new Item(itemData, amount));
        }

        Debug.Log(
            $"Inventory: Added {amount} {itemData.displayName}"
        );

        return true;
    }

    // Remove items only if enough are owned
    public bool RemoveItem(ItemData itemData, int amount)
    {
        if (Data == null || itemData == null || amount <= 0)
        {
            Debug.LogWarning("Inventory: Invalid RemoveItem request.");
            return false;
        }

        Item existing = Data.inventoryItemIds.Find(
            item => item != null && item.data == itemData
        );

        if (existing == null ||
            existing.quantity < amount ||
            existing.quantity < 0)
        {
            Debug.LogWarning(
                "Inventory: Insufficient item quantity."
            );
            return false;
        }

        existing.quantity -= amount;

        if (existing.quantity == 0)
            Data.inventoryItemIds.Remove(existing);

        Debug.Log(
            $"Inventory: Removed {amount} {itemData.displayName}"
        );

        return true;
    }

    public void InitData()
    {
        if (data_initialized)
            return;

        Data = new PlayerData();
    }

    public bool TrySetPlayerLevel(int newLevel)
    {
        if (newLevel < 1)
        {
            Debug.LogWarning(
                $"Invalid player level: {newLevel}"
            );

            return false;
        }

        Data.level = newLevel;

        return true;
    }
}
