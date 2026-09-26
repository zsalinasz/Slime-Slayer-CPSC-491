using UnityEngine;

public enum AIState
{
    Idle,
    Chase,
    Attack,
    Recovery,
    HitReaction,
    Dead
}

public class EnemyAIController : MonoBehaviour
{
    //Giving enemy controller a direct reference to player position (transform). Might change in the future.
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 2.5f;

    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float moveDistance = 5.0f;
    [SerializeField] private AIState currentState = AIState.Idle;

    private SpriteRenderer spriteRenderer;

    private Vector3 startPosition;
    private int direction = 1;

    private void Start()
    {
        startPosition = transform.position;
	spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        UpdateColor();

	float distanceToPlayer = Vector2.Distance(transform.position, player.position);
	if (distanceToPlayer < detectionRange)
	{
	    currentState = AIState.Chase;
	    if (distanceToPlayer < attackRange)
	    {
	        currentState = AIState.Attack;
	    }
	}
	else
	{
	    currentState = AIState.Idle;
	}

	switch (currentState)
	{
	    case AIState.Idle:
		    HandleIdle();
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
        MoveBackAndForth();
    }

    private void HandleChase()
    {
        MoveTowardPlayer();
    }

    private void HandleAttack()
    {
    }

    private void HandleRecovery()
    {
    }

    private void HandleHitReaction()
    {
    }

    private void HandleDead()
    {
    }

    private void MoveBackAndForth()
    {
        transform.position +=
            Vector3.right * direction * moveSpeed * Time.deltaTime;

        float distanceFromStart =
            transform.position.x - startPosition.x;

        if (distanceFromStart >= moveDistance)
        {
            direction = -1;
        }
        else if (distanceFromStart <= -moveDistance)
        {
            direction = 1;
        }
    }


    private void MoveTowardPlayer()
    {
//        private int dx = 0, dy = 0;
//	dx = player.position.x - transform.position.x;
//	dy = player.positiony - transform.position.y;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
	Vector2 vecDirection = (player.position - transform.position).normalized;

        transform.position += (Vector3)(vecDirection * moveSpeed * Time.deltaTime);

    }


    private void UpdateColor()
    {
        switch (currentState)
        {
        case AIState.Idle:
            spriteRenderer.color = Color.white;
            break;

        case AIState.Chase:
            spriteRenderer.color = Color.yellow;
            break;

        case AIState.Attack:
            spriteRenderer.color = Color.red;
            break;

        case AIState.Recovery:
            spriteRenderer.color = Color.blue;
            break;

        case AIState.HitReaction:
            spriteRenderer.color = Color.magenta;
            break;

        case AIState.Dead:
            spriteRenderer.color = Color.gray;
            break;
        }
    }
}
