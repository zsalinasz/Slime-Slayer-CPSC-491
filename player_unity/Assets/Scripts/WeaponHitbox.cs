using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    
    [Header("Hit Effect")]
    [SerializeField] private float hitEffectSize = 0.15f;
    [SerializeField] private float hitEffectLifetime = 0.15f;
    [SerializeField] private string hitEffectSortingLayer = "Player";

    [SerializeField] public GameObject hitEffectPrefab;
    
    public float DamageMultiplier { get; set; } = 1f;
    
    public int Damage => Mathf.RoundToInt(damage * DamageMultiplier);
    
    private readonly HashSet<IsDamageable> hitThisSwing = new HashSet<IsDamageable>();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
    	if (other.TryGetComponent(out IsDamageable target) && !hitThisSwing.Contains(target))
    	{
    		target.TakeDamage(Damage);
    		hitThisSwing.Add(target);
    		
    		SpawnEffect(other.ClosestPoint(transform.position));
    	}
    }
    
    public void ResetSwing()
    {
    	hitThisSwing.Clear();
    }
    
    public void PlaySwingEffect()
    {
    	SpawnEffect(transform.position);
    }
    
    private void SpawnEffect(Vector2 position)
    {
        GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
    	Destroy(effect, hitEffectLifetime);
    }
}
