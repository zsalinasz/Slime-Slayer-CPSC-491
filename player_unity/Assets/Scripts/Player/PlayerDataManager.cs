using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    public PlayerData Data {get; private set; }

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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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



        //Add some items to player's inventory:
        Item potion = new Item("Health Potion", "health_potion");
        Item sword = new Item("Iron Sword", "iron_sword");
        Data.inventoryItemIds.Add(potion);
        Data.inventoryItemIds.Add(sword);
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
