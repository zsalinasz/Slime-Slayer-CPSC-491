using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class InnkeeperDialogue : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private Transform player;

    private List<OptionActionPair> options;

    [SerializeField] public float interactionDistance;
    private float distance = 0.0f;

    bool atTop = false;

    public void Start()
    { 
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("NPCDialogue: Player not found.");
        }

        OptionActionPair option1 = new OptionActionPair("Tell me about the Slime Legion.", TellAboutSlimes);
        OptionActionPair option2 = new OptionActionPair("Do you have any items to sell?", OfferStore);
        OptionActionPair option3 = new OptionActionPair("This is a test option.", TestOption);
        
        options = new List<OptionActionPair>();
        options.Add(option1);
        options.Add(option2);
        options.Add(option3);

        dialogueController.CreateButtons(options);

    }

    public void Update()
    {
        distance = Vector2.Distance(transform.position, player.position);
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (distance < interactionDistance && dialogueController.dialoguePanel.activeSelf == false)
            {
                Interact();
            }
        }

    }

    public void Interact()
    {
        dialogueController.ShowDialogue(
            "Innkeeper",
            "Welcome to Slime Slayer! Our world has been overrun by the Slime Legion!",
            options
        );
    }

    private void TellAboutSlimes()
    {
        dialogueController.ShowMessage(
            "Innkeeper",

            "The slimes have been growing increasingly " +
            "aggressive. Travelers are afraid to leave town!",

            dialogueController.CloseDialogue
        );
    }

    private void OfferStore()
    {
        dialogueController.ShowMessage(
            "Innkeeper",

            "Of course! Take a look at my wares.",

            OpenStore
        );
    }

    private void TestOption()
    {
        dialogueController.ShowMessage(
            "Innkeeper",
            "Test received. It works?",
            dialogueController.CloseDialogue
            );
    }

    private void OpenStore()
    {
        dialogueController.CloseDialogue();

        // Next step: Open the merchant interface.
    }
}
