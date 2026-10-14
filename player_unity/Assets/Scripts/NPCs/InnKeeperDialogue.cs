using UnityEngine;
using UnityEngine.InputSystem;

public class InnkeeperDialogue : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;

    public void Update()
    {

    }

    public void Interact()
    {
        dialogueController.ShowDialogue(
            "Innkeeper",

            "Welcome to Slime Slayer! Our world has been " +
            "overrun by the Slime Legion!",

            "Tell me about the Slime Legion.",
            TellAboutSlimes,

            "Do you have any items to sell?",
            OfferStore
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

    private void OpenStore()
    {
        dialogueController.CloseDialogue();

        // Next step: Open the merchant interface.
    }
}
