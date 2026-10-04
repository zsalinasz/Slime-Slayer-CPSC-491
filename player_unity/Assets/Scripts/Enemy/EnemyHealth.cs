using UnityEngine;

public class EnemyHealth : MonoBehaviour, IsDamageable
{

    [SerializeField] private int health = 100;
    bool hasManagerInstance = false;

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
        health -= damage;

	Debug.Log($"Enemy took {damage} damage. Health: {health}");

	if (health <= 0)
	{
            Die();
	}

    }
    private void Die()
    {
        Destroy(gameObject);
	if (hasManagerInstance)
	{
	    EnemyManager.Instance.enemyDead = true;
	}
    }
    
}
