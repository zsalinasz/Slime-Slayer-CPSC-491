using UnityEngine;

public class EnemyHealth : MonoBehaviour, IsDamageable
{
    [SerializeField] private int health = 100;

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
    }
    
}
