using UnityEngine;

public class EnemyHealth : MonoBehaviour, IsDamageable
{

    [SerializeField] private int health = 100;
    [SerializeField] private GameObject deathEffectPrefab;
    bool hasManagerInstance = false;
    private EnemyAIController controllerInstance;

    private void Awake()
    {
	//Since the EnemyAIController is on the same GameObject as this EnemyHealth script, this line will search the GameObject (i.e. the current instance of the enemy prefab) and save the specific link we need to communicate action between the enemy and its controller.
	controllerInstance = GetComponent<EnemyAIController>();
    }

    void Start()
    {
	if (EnemyManager.Instance == null)
	{
	    Debug.LogError("EnemyManager doesn't exist.");
	}
	else
	{
            hasManagerInstance = true;
	}
    }

    public void TakeDamage(int damage)
    {

	if (controllerInstance != null)
	{
	    //Let the controller know Enemy has been hit.
	    //Possibly in future, we can communicate more about the severity of the hit, to further mediate behavior.
	    //Also, regardless of how complex hit reactions get, I really think this line should be replaced with some kind of function call to the controller instead.
	    controllerInstance.tookHit = true;
	}

        health -= damage;

	Debug.Log($"Enemy took {damage} damage. Health: {health}");

	if (health <= 0)
	{
            Die();
	}

    }
    private void Die()
    {
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
	if (hasManagerInstance)
	{
	    EnemyManager.Instance.enemyDead = true;
	}
    }
    
}
