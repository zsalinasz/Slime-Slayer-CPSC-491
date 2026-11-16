
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewQuest",
    menuName = "Quests/Quest Data"
)]
public class QuestData : ScriptableObject
{
    [Header("Quest Information")]
    public string questId;
    public string questName;

    [TextArea(3, 5)]
    public string description;

    [Header("Quest Objective")]
    public string targetEnemyId;

    [Min(1)]
    public int requiredKills = 5;

    [Header("Quest Reward")]
    [Min(0)]
    public int currencyReward = 100;
}
