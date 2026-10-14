using UnityEngine;
using UnityEngine.InputSystem;
public class temp_dialogue_listener : MonoBehaviour
{

    [SerializeField] public DialogueUI dialogueUI;
    private bool dialogue_showing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
	        Debug.Log("E-key detected");
            if (!dialogue_showing)
            {
                dialogueUI.ShowDialogue("Test Name", "Welcome to Slime Slayer");
                dialogue_showing = true;
            }
            else
            {
                dialogueUI.HideDialogue();
                dialogue_showing = false;
            }
        }

    }
}
