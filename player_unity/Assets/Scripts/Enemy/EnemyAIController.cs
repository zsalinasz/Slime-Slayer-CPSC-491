using UnityEngine;

public enum AIState
{
    Idle,
    Chase,
    Engage,
    Attack,
    Recovery,
    HitReaction,
    Dead
}

[RequireComponent(typeof(EnemyMovement))]
public class EnemyAIController : MonoBehaviour
{
    [Header("Targets and ranges")]
    [SerializeField] private Transform player;
    [SerializeField, Min(0f)] private float detectionRange = 5f;
    [SerializeField, Min(0f)] private float attackRange = 2.5f;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 2f;
    [SerializeField, Min(0f)] private float engageMoveSpeed = 7f;
    [SerializeField, Min(0f)] private float moveDistance = 5f;
    [SerializeField] private Vector3 unit_velocity; // Direction only; kept for Inspector compatibility.

    [Header("Engagement")]
    [SerializeField, Min(0f)] private float engagementDuration = 2f;
    [SerializeField, Range(0f, 89f)] private float jaggedAngle = 60f;
    [SerializeField, Min(0.01f)] private float jaggedSwitchInterval = 0.45f;

    [Header("Hit reaction")]
    [SerializeField, Min(0f)] private float knockbackSpeedMultiplier = 3f;
    [SerializeField, Min(0f)] private float hitReactionDuration = 0.12f;

    [Header("State and visuals")]
    [SerializeField] private AIState currentState = AIState.Idle;
    [SerializeField] private EnemyAnimationController animationController;

    private EnemyMovement movement;
    private SpriteRenderer spriteRenderer;
    private Vector3 patrolStartPosition;
    private int patrolDirection = 1;
    private float engageElapsed;
    private float hitElapsed;
    private bool engagementFinished;
    private Vector3 knockbackDirection;
    private float currentMoveSpeed;

    public AIState CurrentState => currentState;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animationController == null)
            animationController = GetComponent<EnemyAnimationController>();
    }

    private void Start()
    {
        patrolStartPosition = transform.position;
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player == null)
            Debug.LogWarning("EnemyAIController: no Player target found.", this);
    }

    private void Update()
    {
        // Each state chooses a fresh direction: no stale velocity leaks into other states.
        unit_velocity = Vector3.zero;
        currentMoveSpeed = moveSpeed;

        if (currentState != AIState.Dead)
        {
            EvaluateState(player == null ? float.PositiveInfinity : calcDistanceToPlayer(player));
            HandleCurrentState();
        }

        movement.Move(unit_velocity, currentMoveSpeed);
        if (animationController != null)
            animationController.UpdateMovement(movement.Velocity);
        UpdateColor();
    }

    // Retained for existing callers that use the original method names.
    public AIState getCurrentState() => currentState;
    public void setPlayerTransform(Transform playerTransform) => player = playerTransform;

    public float calcDistanceToPlayer(Transform playerTransform)
    {
        if (playerTransform == null)
            return float.PositiveInfinity;
        return Vector2.Distance(transform.position, playerTransform.position);
    }

    public void NotifyHit()
    {
        if (currentState == AIState.Dead)
            return;

        hitElapsed = 0f;
        knockbackDirection = player != null
            ? (transform.position - player.position).normalized
            : Vector3.zero;
        currentState = AIState.HitReaction;
    }

    public void NotifyDeath()
    {
        currentState = AIState.Dead;
        movement.Stop();
        unit_velocity = Vector3.zero;
    }

    public void EvaluateState(float distanceToPlayer)
    {
        // Transient states take priority over distance-based transitions.
        if (currentState == AIState.Dead || currentState == AIState.HitReaction || currentState == AIState.Recovery)
            return;

        if (distanceToPlayer >= detectionRange)
        {
            currentState = AIState.Idle;
            ResetEngagement();
            return;
        }

        if (distanceToPlayer < attackRange)
        {
            currentState = AIState.Attack;
            return;
        }

        if (engagementFinished)
        {
            currentState = AIState.Chase;
            return;
        }

        currentState = AIState.Engage;
    }

    public void HandleCurrentState()
    {
        switch (currentState)
        {
            case AIState.Idle:
                HandleIdle();
                break;
            case AIState.Engage:
                HandleEngage();
                break;
            case AIState.Chase:
                HandleChase();
                break;
            case AIState.Attack:
                HandleAttack();
                break;
            case AIState.Recovery:
                HandleRecovery();
                break;
            case AIState.HitReaction:
                HandleHitReaction();
                break;
            case AIState.Dead:
                HandleDead();
                break;
        }
    }

    private void HandleIdle()
    {
        // Patrol uses an anchor at each turn, preserving the original back-and-forth behavior.
        if (Vector2.Distance(transform.position, patrolStartPosition) >= moveDistance)
        {
            patrolDirection *= -1;
            patrolStartPosition = transform.position;
        }
        unit_velocity = Vector3.right * patrolDirection;
    }

    private void HandleChase()
    {
        if (player != null)
            unit_velocity = (player.position - transform.position).normalized;
    }

    private void HandleEngage()
    {
        if (player == null)
            return;

        currentMoveSpeed = engageMoveSpeed;
        engageElapsed += Time.deltaTime;

        // Alternate a rotated approach direction every interval, rather than
        // using a counter that never changes.
        int side = Mathf.FloorToInt(engageElapsed / jaggedSwitchInterval) % 2 == 0 ? 1 : -1;
        Vector3 towardPlayer = player.position - transform.position;
        unit_velocity = Quaternion.Euler(0f, 0f, side * jaggedAngle) * towardPlayer.normalized;

        if (engageElapsed >= engagementDuration)
        {
            engagementFinished = true;
            currentState = AIState.Chase;
        }
    }

    private void HandleAttack()
    {
        // Attack execution remains a placeholder for the existing combat integration.
        // The enemy now stops instead of drifting at its previous velocity.
    }

    private void HandleRecovery()
    {
        // Reserved for an attack recovery mechanic.
    }

    private void HandleHitReaction()
    {
        currentMoveSpeed = moveSpeed * knockbackSpeedMultiplier;
        unit_velocity = knockbackDirection;
        hitElapsed += Time.deltaTime;
        if (hitElapsed >= hitReactionDuration)
            currentState = AIState.Idle; // EvaluateState will select the next combat state on the next frame.
    }

    private void HandleDead()
    {
        // EnemyHealth handles the death effect, destruction, and manager notification.
    }

    private void ResetEngagement()
    {
        engageElapsed = 0f;
        engagementFinished = false;
    }

    private void UpdateColor()
    {
        if (spriteRenderer == null)
            return;

        switch (currentState)
        {
            case AIState.Idle: spriteRenderer.color = Color.white; break;
            case AIState.Engage: spriteRenderer.color = Color.cyan; break;
            case AIState.Chase: spriteRenderer.color = Color.yellow; break;
            case AIState.Attack: spriteRenderer.color = Color.red; break;
            case AIState.Recovery: spriteRenderer.color = Color.blue; break;
            case AIState.HitReaction: spriteRenderer.color = Color.magenta; break;
            case AIState.Dead: spriteRenderer.color = Color.gray; break;
        }
    }
}
