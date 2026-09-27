using NUnit.Framework;
using UnityEngine;

public class EnemyStateTests
{
    private GameObject enemyObject;
    private EnemyAIController enemy;

    private GameObject playerHolder;
    private Transform playerTransform;

    [SetUp]
    public void SetUp()
    {
        enemyObject = new GameObject("Enemy");
        
	enemy = enemyObject.AddComponent<EnemyAIController>();

	enemyObject.transform.position = Vector3.zero;

	playerHolder = new GameObject("PlayerTarget");
	playerTransform = playerHolder.transform;
	playerTransform.position = new Vector3(10f, 0f, 0f);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyObject);
	Object.DestroyImmediate(playerHolder);
    }

    [Test]
    public void EnemyOutsideDetectionRange_DoesNotChase()
    {
        float distanceToPlayer = enemy.calcDistanceToPlayer(playerTransform);
        enemy.EvaluateState(distanceToPlayer);

	Assert.AreNotEqual(AIState.Chase, enemy.getCurrentState());
        Assert.AreEqual(AIState.Idle, enemy.getCurrentState());
    }

    [Test]
    public void EnemyInsideDetectionRange_TransitionsToChase()
    {
	    playerTransform.position = new Vector3(2.5f, 2.5f, 0f);
	    float distanceToPlayer = enemy.calcDistanceToPlayer(playerTransform);
	    enemy.EvaluateState(distanceToPlayer);

	    Assert.AreEqual(AIState.Chase, enemy.getCurrentState());
    }

    [Test]
    public void EnemyInsideAttackRange_TransitionsToAttack()
    {
	    playerTransform.position = new Vector3(1.0f, 0f, 0f);
	    float distanceToPlayer = enemy.calcDistanceToPlayer(playerTransform);
	    enemy.EvaluateState(distanceToPlayer);

	    Assert.AreEqual(AIState.Attack, enemy.getCurrentState());
    }

}
