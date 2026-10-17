using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continuePrompt;

    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private Button choiceButton1;
    [SerializeField] private Button choiceButton2;

    private Action onContinue;
    private bool waitingForContinue;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (waitingForContinue && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            waitingForContinue = false;

            Action callback = onContinue;
            onContinue = null;

            callback?.Invoke();
        }
    }

    public void ShowDialogue(
        string speaker,
        string message,
        string option1,
        Action action1,
        string option2,
        Action action2)
    {
        dialoguePanel.SetActive(true);

        speakerName.text = speaker;
        dialogueText.text = message;

        choicesContainer.SetActive(true);
        continuePrompt.gameObject.SetActive(false);

        waitingForContinue = false;

        ConfigureButton(choiceButton1, option1, action1);
        ConfigureButton(choiceButton2, option2, action2);
    }

    private void ConfigureButton(
        Button button,
        string label,
        Action action)
    {
        button.GetComponentInChildren<TMP_Text>().text = label;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() => action?.Invoke());
    }

    public void ShowMessage(
        string speaker,
        string message,
        Action callback)
    {
        dialoguePanel.SetActive(true);

        speakerName.text = speaker;
        dialogueText.text = message;

        choicesContainer.SetActive(false);

        continuePrompt.text = "Press E to continue";
        continuePrompt.gameObject.SetActive(true);

        onContinue = callback;
        waitingForContinue = true;
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        waitingForContinue = false;
        onContinue = null;
    }
}
