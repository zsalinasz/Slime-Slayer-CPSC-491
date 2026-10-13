
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NPCPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private float waitTime = 1f;

    private Rigidbody2D rb;
    private Transform target;
    private float waitTimer;

    private bool isTalking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        target = pointB;
    }

    private void FixedUpdate()
    {
        if (isTalking)
            return;

        if (pointA == null || pointB == null)
            return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = target.position;

        Vector2 difference = targetPosition - currentPosition;
        float distance = difference.magnitude;

        if (distance <= stoppingDistance)
        {
            target = target == pointA ? pointB : pointA;
            waitTimer = waitTime;
            return;
        }

        float step = moveSpeed * Time.fixedDeltaTime;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            step
        );

        rb.MovePosition(nextPosition);
    }
    public void SetTalking(bool talking)
    {
        isTalking = talking;
    }
}

