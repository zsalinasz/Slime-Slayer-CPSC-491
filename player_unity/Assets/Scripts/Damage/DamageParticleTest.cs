using UnityEngine;

public class DamageOnContactParticleTest : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private GameObject hitEffectPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerHealth health))
        {
            health.TakeDamage(damage, transform.position);

            Instantiate(
                hitEffectPrefab,
                other.transform.position,
                Quaternion.identity
            );
        }
    }
}