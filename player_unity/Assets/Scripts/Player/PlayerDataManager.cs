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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
