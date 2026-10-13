
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowDialogue(string npcName, string message)
    {
        npcNameText.text = npcName;
        dialogueText.text = message;

        dialoguePanel.SetActive(true);
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
