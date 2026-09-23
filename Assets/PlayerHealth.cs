using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
	[Header("Health")]
	[SerializeField] private int maxHealth = 3;
	
	[Header("Damage")]
	[SerializeField] private float invincibilityTime = 1f;
	[SerializeField] private float flashInterval = 0.1f;
	
	[Header("Knockback")]
	[SerializeField] private float knockbackSpeed = 3f;
	[SerializeField] private float knockbackDuration = 0.15f;
	
	[Header("Death & Respawn")]
	[SerializeField] private float respawnDelay = 2f;
	[SerializeField] private Transform respawnPoint;
	[Tooltip("Movement/input scripts to turn off whole dead")]
	[SerializeField] private Behaviour[] disableWhileDead;
	
	public UnityEvent<int, int> OnHealthChanged;
	public UnityEvent OnDied;
	public UnityEvent OnRespawned;
	
	private int currentHealth;
	private bool isInvincible;
	private bool isDead;
	private Rigidbody2D rb;
	private SpriteRenderer spriteRenderer;
	private Coroutine knockbackRoutine;
	
	public bool IsDead => isDead;
	
	private void Awake() 
	{
		rb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		currentHealth = maxHealth;
	}
	
	private void Start()
	{
		if (respawnPoint == null)
		{
			GameObject point = new GameObject("PlayerRespawnPoint");
			point.transform.position = transform.position;
			respawnPoint = point.transform;
		}
		
		OnHealthChanged?.Invoke(currentHealth, maxHealth);
	}
	
	// damage from attacking enemies
	public void TakeDamage(int amount, Vector2 sourcePosition)
	{
		if (isDead || isInvincible)
			return;
			
		ApplyDamage(amount);
		
		if (!isDead)
		{
			Vector2 direction = (Vector2)transform.position - sourcePosition;
			if (direction.sqrMagnitude < 0.0001f)
				direction = Vector2.up;
				
			knockbackRoutine = StartCoroutine(knockback(direction.normalized));
		}
	}
	
	// damage from status effect or environment
	public void TakeDamage(int amount)
	{
		if (isDead || isInvincible)
			return;
			
		ApplyDamage(amount);
	}
	
	public void ApplyDamage(int amount)
	{
		if (isDead || isInvincible)
			return;
			
		currentHealth = Mathf.Max(currentHealth - amount, 0);
		OnHealthChanged?.Invoke(currentHealth, maxHealth);
		
		if (currentHealth <= 0)
			StartCoroutine(DieAndRespawn());
			
		else
			StartCoroutine(InvincibilityFrames());
	}
	
	public void Heal(int amount)
	{
		if (isDead)
			return;
			
		currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
		OnHealthChanged?.Invoke(currentHealth, maxHealth);
	}
	
	public void SetRespawnPoint(Transform newPoint)
	{
		respawnPoint = newPoint;
	}
	
	private IEnumerator knockback(Vector2 direction)
	{
		SetControlEnabled(false);
		rb.linearVelocity = direction * knockbackSpeed;
		
		yield return new WaitForSeconds(knockbackDuration);
		
		rb.linearVelocity = Vector2.zero;
		SetControlEnabled(true);
		knockbackRoutine = null;
	}
	
	private IEnumerator InvincibilityFrames()
	{
		isInvincible = true;
		
		float elapsed = 0f;
		while (elapsed < invincibilityTime)
		{
			if (spriteRenderer != null)
				spriteRenderer.enabled = !spriteRenderer.enabled;
				
			yield return new WaitForSeconds(flashInterval);
			elapsed += flashInterval;
		}
		
		if (spriteRenderer != null)
			spriteRenderer.enabled = true;
		
		isInvincible = false;
	}
	
	private IEnumerator DieAndRespawn()
	{
		isDead = true;
		OnDied?.Invoke();
		
		if (knockbackRoutine != null)
		{
			StopCoroutine(knockbackRoutine);
			knockbackRoutine = null;
		}
		
		SetControlEnabled(false);
		rb.linearVelocity = Vector2.zero;
		
		if (spriteRenderer != null)
			spriteRenderer.enabled = false;
			
		yield return new WaitForSeconds(respawnDelay);
		
		rb.position = respawnPoint.position;
		transform.position = respawnPoint.position;
		
		currentHealth = maxHealth;
		OnHealthChanged?.Invoke(currentHealth, maxHealth);
		
		if (spriteRenderer != null)
			spriteRenderer.enabled = true;
			
		SetControlEnabled(true);
		isDead = false;
		
		OnRespawned?.Invoke();
		
		StartCoroutine(InvincibilityFrames());
	}
	
	private void SetControlEnabled(bool enabled)
	{
		foreach (Behaviour b in disableWhileDead)
			if (b != null)
				b.enabled = enabled;
	}
    
}
