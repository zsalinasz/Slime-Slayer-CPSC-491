using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OnQuestStarted(string questId)
    {
        Debug.Log("Quest started: " + questId);
    }

    public void OnQuestCompleted(string questId)
    {
        Debug.Log("Quest completed: " + questId);
    }

    public void OnWorldInteraction(string interactionId)
    {
        Debug.Log("World interaction: " + interactionId);
    }
}