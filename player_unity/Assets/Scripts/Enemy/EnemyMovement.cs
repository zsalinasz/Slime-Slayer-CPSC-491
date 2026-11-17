using UnityEngine;

/// <summary>
/// Applies movement requested by the AI. Decisions about where to go stay in EnemyAIController.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    public Vector3 Velocity { get; private set; }

    public void Move(Vector3 direction, float speed)
    {
        Velocity = direction.sqrMagnitude > 0f ? direction.normalized * Mathf.Max(0f, speed) : Vector3.zero;
        transform.position += Velocity * Time.deltaTime;
    }

    public void Stop()
    {
        Velocity = Vector3.zero;
    }
}
