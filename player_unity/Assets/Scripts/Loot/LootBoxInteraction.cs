
using UnityEngine;
using UnityEngine.InputSystem;

public class LootBoxInteraction : MonoBehaviour
{
    [SerializeField] private LootBox lootBox;
    [SerializeField] private LootBoxUI lootBoxUI;
    [SerializeField] private Transform player;
    [SerializeField] private float interactionRange = 2f;

    private void Update()
    {
        if (player == null || lootBox == null ||
            lootBoxUI == null || Keyboard.current == null)
            return;

        if (!Keyboard.current.eKey.wasPressedThisFrame)
            return;

        float distance = Vector2.Distance(
            player.position, transform.position
        );

        if (lootBoxUI.IsOpen)
        {
            lootBoxUI.Close();
            return;
        }

        if (distance <= interactionRange)
            lootBoxUI.Open(lootBox);
    }
}
