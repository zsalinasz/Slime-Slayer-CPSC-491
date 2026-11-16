
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestGiverDialogue : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private Transform player;
    [SerializeField] private QuestManager questManager;

    [Header("NPC Settings")]
    [SerializeField] private string npcName = "Village Elder";
    [SerializeField] private float interactionDistance = 2f;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (questManager == null)
            questManager = QuestManager.Instance;
    }

    private void Update()
    {
        if (player == null ||
            dialogueController == null ||
            questManager == null ||
            Keyboard.current == null)
            return;

        if (!Keyboard.current.eKey.wasPressedThisFrame)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= interactionDistance &&
            !dialogueController.dialoguePanel.activeSelf)
        {
            Interact();
        }
    }

    public void Interact()
    {
        switch (questManager.State)
        {
            case QuestState.Available:
                OfferQuest();
                break;

            case QuestState.Active:
                ShowProgress();
                break;

            case QuestState.ReadyToTurnIn:
                OfferTurnIn();
                break;

            case QuestState.Completed:
                dialogueController.ShowMessage(
                    npcName,
                    "Thank you again for protecting our village!",
                    dialogueController.CloseDialogue
                );
                break;
        }
    }

    private void OfferQuest()
    {
        List<OptionActionPair> options =
            new List<OptionActionPair>()
            {
                new OptionActionPair(
                    "Accept Quest",
                    AcceptQuest
                ),

                new OptionActionPair(
                    "Decline",
                    DeclineQuest
                )
            };

        dialogueController.ShowDialogue(
            npcName,
            "Slimes have been attacking our village! " +
            "Could you defeat five of them? " +
            "I'll reward you with 100 gold.",
            options
        );
    }

    private void AcceptQuest()
    {
        bool accepted = questManager.AcceptQuest();

        dialogueController.ShowMessage(
            npcName,
            accepted
                ? "Thank you! Return when you've defeated the slimes."
                : "I cannot offer that quest right now.",
            dialogueController.CloseDialogue
        );
    }

    private void DeclineQuest()
    {
        dialogueController.ShowMessage(
            npcName,
            "I understand. Come back if you change your mind.",
            dialogueController.CloseDialogue
        );
    }

    private void ShowProgress()
    {
        QuestData quest = questManager.CurrentQuest;

        dialogueController.ShowMessage(
            npcName,
            $"You've defeated {questManager.CurrentKills}" +
            $"/{quest.requiredKills} slimes. Keep going!",
            dialogueController.CloseDialogue
        );
    }

    private void OfferTurnIn()
    {
        List<OptionActionPair> options =
            new List<OptionActionPair>()
            {
                new OptionActionPair(
                    "Collect Reward",
                    CollectReward
                ),

                new OptionActionPair(
                    "Not Yet",
                    dialogueController.CloseDialogue
                )
            };

        dialogueController.ShowDialogue(
            npcName,
            "Excellent work! You've defeated all the slimes. " +
            "Are you ready to collect your reward?",
            options
        );
    }

    private void CollectReward()
    {
        bool success = questManager.TurnInQuest();

        dialogueController.ShowMessage(
            npcName,
            success
                ? "Thank you! Here is your payment."
                : "I couldn't process your reward.",
            dialogueController.CloseDialogue
        );
    }
}
