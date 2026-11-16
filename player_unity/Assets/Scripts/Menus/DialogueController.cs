
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] public GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continuePrompt;

    [Header("Dialogue Choices")]
    [SerializeField] private GameObject choicesContainer;

    [SerializeField]
    private List<Button> choiceButtons =
        new List<Button>();

    private Action onContinue;
    private bool waitingForContinue;

    private void Awake()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (waitingForContinue &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
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
        List<OptionActionPair> options)
    {
        dialoguePanel.SetActive(true);

        speakerName.text = speaker;
        dialogueText.text = message;

        choicesContainer.SetActive(true);
        continuePrompt.gameObject.SetActive(false);

        waitingForContinue = false;
        onContinue = null;

        CreateButtons(options);
    }

    public void CreateButtons(List<OptionActionPair> options)
    {
        // Remove previously generated buttons
        foreach (Button button in choiceButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
                Destroy(button.gameObject);
            }
        }

        choiceButtons.Clear();

        if (options == null)
            return;

        foreach (OptionActionPair option in options)
        {
            CreateChoiceButton(
                option.option,
                option.action
            );
        }
    }

    private void CreateChoiceButton(
        string label,
        Action action)
    {
        GameObject buttonObj = new GameObject(
            "ChoiceButton",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button)
        );

        // Use the assigned choices container
        buttonObj.transform.SetParent(
            choicesContainer.transform,
            false
        );

        RectTransform rectTransform =
            buttonObj.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(250, 50);

        GameObject textObj = new GameObject(
            "ButtonText",
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

        textObj.transform.SetParent(
            buttonObj.transform,
            false
        );

        TextMeshProUGUI textComponent =
            textObj.GetComponent<TextMeshProUGUI>();

        textComponent.text = label;
        textComponent.fontSize = 18;
        textComponent.alignment =
            TextAlignmentOptions.Center;
        textComponent.color = Color.black;

        RectTransform textRect =
            textObj.GetComponent<RectTransform>();

        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Button choiceButton =
            buttonObj.GetComponent<Button>();

        choiceButton.onClick.AddListener(
            () => action?.Invoke()
        );

        choiceButtons.Add(choiceButton);
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
