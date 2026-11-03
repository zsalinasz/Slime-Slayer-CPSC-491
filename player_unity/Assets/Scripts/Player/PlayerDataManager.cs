
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
