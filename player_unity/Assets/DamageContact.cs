using UnityEngine;

public class DamageOnContact : MonoBehaviour
{
 	[SerializeField] private int damage = 1;
 	
 	private void OnTriggerStay2D(Collider2D other)
 	{
 		if (other.TryGetComponent(out PlayerHealth health))
 			health.TakeDamage(damage, transform.position);
 	}
}
