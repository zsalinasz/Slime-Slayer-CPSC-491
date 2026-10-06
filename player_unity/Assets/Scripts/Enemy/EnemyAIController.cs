using UnityEngine;
using System;

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

    //Use before referencing the manager to ensure no run-time errors.
    bool hasManagerInstance = false;
    //A simple predicate to guide state transitions.
    public bool tookHit = false;
    int knockBackTicks = 0;

    public bool beginEngage = false;
    int engagementTicks = 0;

    private void Start()
    {
        startPosition = transform.position;
	spriteRenderer = GetComponent<SpriteRenderer>();
	if (player == null)
	        player = GameObject.FindGameObjectWithTag("Player").transform;
	if (EnemyManager.Instance == null)
	{
            Debug.LogError("EnemyManager doesn't exist.");
	}
	else
	{
	    hasManagerInstance = true;
	}
    }

    private void Update()
    {
        UpdateColor();

	float distanceToPlayer = calcDistanceToPlayer(player); 
	EvaluateState(distanceToPlayer);
	HandleCurrentState();
    }

    public AIState getCurrentState()
    {
        return currentState;
    }
    public void setPlayerTransform(Transform playerTransform)
    {
        player = playerTransform;
    }

    public float calcDistanceToPlayer(Transform playerTransform)
    {
        return Vector2.Distance(transform.position, playerTransform.position);
    }

    public void EvaluateState(float distanceToPlayer)
    {
        
        if (tookHit)
	{
	    currentState = AIState.HitReaction;
	    //Return before evaluating the rest because taking a hit supercedes other concerns.
	    return;
	}

	if (distanceToPlayer < detectionRange)
	{
	    //currentState = AIState.Chase;
	    
	    //The enemy detects the player.
	    //First, we find out if it is not currently chasing(aka charging) the player and hasn't started engagement yet.
	    if (!beginEngage && currentState != AIState.Chase)
	    {
		//If so, we begin engagement and note that with a flag.
		beginEngage = true;
		currentState = AIState.Engage;
	    }
	    else if (currentState != AIState.Chase)
	    {
		//If it has begun engagement, we check using engagementTicks whether it should still be in engagement mode. Here the enemy will be in engagement for 100 ticks.
		if (engagementTicks < 100)
		{
		    //If enemy is still engaged, maintain currentState in AIState.Engage
	            currentState = AIState.Engage;
		    engagementTicks++;
		}
		else
		{
		    //If the enemy has completed engagement, reset the engagement state variables and set the AI state to chase.
	            engagementTicks = 0;
		    beginEngage = false;
		    currentState = AIState.Chase;
		}
	    }
	    else if (currentState == AIState.Chase)
	    {
		//If the AI is in chase mode, let the states handler process the action. No state variables need to be adjusted here at this time.
	    }



	    if (distanceToPlayer < attackRange)
	    {
	        currentState = AIState.Attack;
	    }
	}
	else
	{
	    currentState = AIState.Idle;
	}
    }

    public void HandleCurrentState()
    {
         
	switch (currentState)
	{
	    case AIState.Idle:
		    HandleIdle();
		    break;

	    case AIState.Chase:
		    HandleChase();
		    break;
	    case AIState.Engage:
		    HandleEngage();
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

    private void HandleEngage()
    {
	//
	float engagementDistance = 3.0f;
	float dampener = 16.0f;
	double x = player.position.x + engagementDistance * Math.Sin(engagementTicks / dampener);
	double y = player.position.y + engagementDistance * Math.Cos(engagementTicks / dampener);

        Vector3 diff = transform.position;
	diff.x -= (float)x;
	diff.y -= (float)y;

	transform.position -= diff;



    }

    private void HandleAttack()
    {
    }

    private void HandleRecovery()
    {
    }

    private void HandleHitReaction()
    {	
	Vector2 vecDirection = -1*(player.position - transform.position).normalized;

        transform.position += (Vector3)(vecDirection * 3*moveSpeed * Time.deltaTime);
	if (knockBackTicks < 5)
	{
	    knockBackTicks++;
	}
	else
	{
            knockBackTicks = 0;
	    tookHit = false;
	}
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

    private void MoveAwayFromPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
	Vector2 vecDirection = (player.position - transform.position).normalized;

        transform.position += (Vector3)(vecDirection * -1 * moveSpeed * Time.deltaTime);
	
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
