using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyMovementTests 
{
    private GameObject enemyObject;
    private EnemyAIController enemy;

    private GameObject playerHolder;
    private Transform playerTransform;

    [SetUp]
    public void SetUp()
    {
        enemyObject = new GameObject("Enemy");
        enemyObject.AddComponent<SpriteRenderer>();
	enemy = enemyObject.AddComponent<EnemyAIController>();


	enemyObject.transform.position = Vector3.zero;

	playerHolder = new GameObject("PlayerTarget");
	playerTransform = playerHolder.transform;
	playerTransform.position = new Vector3(5f, 0f, 0f);
	enemy.setPlayerTransform(playerTransform);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(enemyObject);
	Object.Destroy(playerHolder);
    }

    [UnityTest]
    public IEnumerator EnemyInChaseState_MovesCloserToPlayer()
    {
        float initialDistance = Vector3.Distance(enemy.transform.position,
			                        playerTransform.position
			);
	yield return new WaitForSeconds(2.5f);

	float finalDistance = Vector3.Distance(enemy.transform.position,
					      playerTransform.position
			);
	Assert.Less(finalDistance, initialDistance);
    }

}
