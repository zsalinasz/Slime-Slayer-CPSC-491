using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            interactionRadius,
            interactableLayer
        );

        if (hits.Length == 0)
        {
            return;
        }

        Collider2D closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            float distance = Vector2.Distance(
                transform.position,
                hit.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = hit;
            }
        }

        if (closest == null)
        {
            return;
        }

        IInteractable interactable =
            closest.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactable.Interact();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            interactionRadius
        );
    }
}