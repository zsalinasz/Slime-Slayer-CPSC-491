using System;
using System.Collections.Generic;

[Serializable]
public class EnemyData
{
    // Identity / Progression
    public string enemyType;
    public int level;

    // Core resources
    public int currentHealth;
    public int maxHealth;

    public int currentStamina;
    public int maxStamina;

    // Enemy stats
    public int strength;
    public int perception;
    public int agility;
    public int endurance;

    // Economy
    public int currency; //For loot dropping


    public EnemyData()
    {
        enemyType = "melee";

	level = 1;

	maxHealth = 100;
	currentHealth = maxHealth;

	maxStamina = 100;
	currentStamina = maxStamina;

	strength = 5;
	perception = 5;
	agility = 5;
	endurance = 5;

	currency = 10;

    }

}
