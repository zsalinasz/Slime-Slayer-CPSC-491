using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Collections.Generic;

public class DialogueController : MonoBehaviour
{
    [SerializeField] public GameObject dialoguePanel;
    [SerializeField] private TMP_Text speakerName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text continuePrompt;

    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private List<Button> choiceButtons;

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
        List<OptionActionPair> options
        //string option1,
        //Action action1,
        //string option2,
        //Action action2
        )
    {
        dialoguePanel.SetActive(true);

        speakerName.text = speaker;
        dialogueText.text = message;

        choicesContainer.SetActive(true);
        continuePrompt.gameObject.SetActive(false);

        waitingForContinue = false;


        //ConfigureButton(choiceButton1, option1, action1);
        //ConfigureButton(choiceButton2, option2, action2);
    }

    public void CreateButtons(List<OptionActionPair> options)
    {
        foreach (OptionActionPair option_action in options)
        {
            CreateChoiceButton(option_action.option, option_action.action);
        }

    }

    private void CreateChoiceButton(
        string label,
        Action action)
    {
        // Create a blank GameObject for the button frame
        GameObject buttonObj = new GameObject("ChoiceButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));

        // Parent it to the choices panel
        GameObject choicesPanel = dialoguePanel.transform.Find("ChoicesPanel").gameObject;
        buttonObj.transform.SetParent(choicesPanel.transform, false);

        // Set the dimensions and positioning
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(250, 50);

        // Configure the text inside the button
        GameObject textObj = new GameObject("ButtonText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(buttonObj.transform, false);

        TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
        textComponent.text = label;
        textComponent.fontSize = 18;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.black;

        //Match text size to button size
        textObj.GetComponent<RectTransform>().sizeDelta = rectTransform.sizeDelta;

        // Hook up the click event listener
        Button choiceButton = buttonObj.GetComponent<Button>();
        choiceButton.onClick.AddListener(() => action?.Invoke());
        
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
