using UnityEngine;
using UnityEngine.InputSystem;

public class InnkeeperDialogue : MonoBehaviour
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private Transform player;

    [SerializeField] public float interactionDistance;
    private float distance = 0.0f;

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
    }

    public void Update()
    {
        distance = Vector2.Distance(transform.position, player.position);
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (distance < interactionDistance)
            {
                Interact();
            }
        }

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
