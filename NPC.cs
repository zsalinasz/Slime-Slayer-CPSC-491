using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private string npcName;

    [TextArea(2, 5)]
    [SerializeField] private string dialogue;

    public void Interact()
    {
        Debug.Log(npcName + ": " + dialogue);
    }
}