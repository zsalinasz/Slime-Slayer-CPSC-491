using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    // Identity / Progression
    public string playerName;
    public int level;
    public int experience;

    // Core resources
    public int currentHealth;
    public int maxHealth;

    public int currentStamina;
    public int maxStamina;

    // Character stats
    public int strength;
    public int perception;
    public int agility;
    public int endurance;

    // Economy
    public int currency;

    // Inventory / perks
    public List<string> inventoryItemIds;
    public List<string> unlockedPerkIds;

    public PlayerData()
    {
        playerName = "Player";

	level = 1;
	experience = 0;

	maxHealth = 100;
	currentHealth = maxHealth;

	maxStamina = 100;
	currentStamina = maxStamina;

	strength = 5;
	perception = 5;
	agility = 5;
	endurance = 5;

	currency = 0;

	inventoryItemIds = new List<string>();
	unlockedPerkIds = new List<string>();
    }

}
