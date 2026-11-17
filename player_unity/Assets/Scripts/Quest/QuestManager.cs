
using System;
using UnityEngine;

public enum QuestState
{
    Available,
    Active,
    ReadyToTurnIn,
    Completed
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [Header("Quest")]
    [SerializeField] private QuestData questData;

    public QuestData CurrentQuest => questData;
    public QuestState State { get; private set; } =
        QuestState.Available;

    public int CurrentKills { get; private set; }

    public event Action OnQuestUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool AcceptQuest()
    {
        if (questData == null ||
            State != QuestState.Available)
            return false;

        CurrentKills = 0;
        State = QuestState.Active;

        Debug.Log("Quest accepted: " + questData.questName);

        OnQuestUpdated?.Invoke();
        return true;
    }

    public void RegisterEnemyKill(string enemyId)
    {
        if (questData == null ||
            State != QuestState.Active ||
            enemyId != questData.targetEnemyId)
            return;

        CurrentKills++;

        if (CurrentKills >= questData.requiredKills)
        {
            CurrentKills = questData.requiredKills;
            State = QuestState.ReadyToTurnIn;

            Debug.Log("Quest ready to turn in!");
        }

        Debug.Log(
            $"Quest progress: {CurrentKills}/{questData.requiredKills}"
        );

        OnQuestUpdated?.Invoke();
    }

    public bool TurnInQuest()
    {
        if (questData == null ||
            State != QuestState.ReadyToTurnIn ||
            PlayerDataManager.Instance == null ||
            PlayerDataManager.Instance.Data == null)
            return false;

        int reward = questData.currencyReward;

        if (reward < 0)
            return false;

        PlayerData player =
            PlayerDataManager.Instance.Data;

        if (player.currency > int.MaxValue - reward)
        {
            Debug.LogWarning(
                "Quest reward would exceed currency limit."
            );
            return false;
        }

        player.currency += reward;
        State = QuestState.Completed;

        Debug.Log($"Quest completed! Earned {reward} gold.");

        OnQuestUpdated?.Invoke();
        return true;
    }
}
