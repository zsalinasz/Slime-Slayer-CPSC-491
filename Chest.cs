using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen;

    public void Interact()
    {
        if (isOpen)
        {
            Debug.Log("Chest is already open.");
            return;
        }

        OpenChest();
    }

    private void OpenChest()
    {
        isOpen = true;

        Debug.Log("Chest opened!");

        // Later:
        // Play animation
        // Give item
        // Update quest
        // Trigger boss transition
    }
}