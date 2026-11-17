using UnityEngine;

public class EnemyHealth : MonoBehaviour, IsDamageable
{
    [SerializeField, Min(1)] private int health = 100;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject hitEffectPrefab;

    // Kept public for compatibility with existing code.
    public int enemy_idx = -1;

    private EnemyAIController controllerInstance;
    private bool isDead;

    private void Awake()
    {
        controllerInstance = GetComponent<EnemyAIController>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        health = Mathf.Max(0, health - damage);
        Debug.Log($"Enemy took {damage} damage. Health: {health}", this);

        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        if (health == 0)
        {
            Die();
            return;
        }

        // Notify AI via a method instead of modifying its internal state flag.
        if (controllerInstance != null)
            controllerInstance.NotifyHit();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        if (controllerInstance != null)
            controllerInstance.NotifyDeath();

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        EnemyManager manager = EnemyManager.Instance;
        if (manager != null)
            manager.NotifyEnemyDied(enemy_idx);

        Destroy(gameObject);
    }
}
