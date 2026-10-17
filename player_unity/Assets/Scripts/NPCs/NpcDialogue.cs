
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCDialogue : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("NPC Information")]
    [SerializeField] private string npcName = "Village Elder";

    [TextArea(2, 5)]
    [SerializeField]
    private string dialogue =
        "Hello adventurer! Can you help our village?";

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 2f;

    private Transform player;
    private NPCPatrol patrol;
    private bool isTalking = false;
    float distance;

    private void Awake()
    {
        patrol = GetComponent<NPCPatrol>();
    }

    private void Start()
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

        Debug.Log($"player: {player.position.x}, {player.position.y}");
        Debug.Log($"player object transform: {playerObject.transform.position.x}, {playerObject.transform.position.y}");
    }

    private void Update()
    {
        if (player == null)
            return;

        distance = Vector2.Distance(
            transform.position,
            player.position
        );
        if (Keyboard.current != null &&
    Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (isTalking)
            {
                EndDialogue();
            }
            else if (distance <= interactionRange)
            {
                
                Debug.Log($"player: {player.position.x}, {player.position.y}");
                Debug.Log($"quest giver: {transform.position.x}, {transform.position.y}");
                Debug.Log($"Distance: {distance}");
                StartDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        if (dialogueUI == null)
        {
            Debug.LogWarning("Dialogue UI not assigned.");
            return;
        }

        isTalking = true;

        if (patrol != null)
            patrol.SetTalking(true);

        dialogueUI.ShowDialogue(npcName, dialogue);
    }

    private void EndDialogue()
    {
        isTalking = false;

        if (dialogueUI != null)
            dialogueUI.HideDialogue();

        if (patrol != null)
            patrol.SetTalking(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            interactionRange
        );
    }
}
