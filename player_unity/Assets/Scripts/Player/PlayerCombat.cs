using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum WeaponType
{
	Sword,
	Spear,
	Bow
}

public class PlayerCombat : MonoBehaviour
{
    [Serializable]
    public class WeaponConfig
    {
    	public WeaponType type;
    	public WeaponHitbox hitbox;
    	public float attackDuration = 0.2f;
    	public float attackCooldown = 0.4f;
    	public float attackDistance = 0.5f;
    	public GameObject hitEffectPrefab;
    	
    	[Header("Directional Hitbox (Optional")]
    	[Tooltip("If set, hitbox's BoxCollider2D is resized to match swing direction - horizontal when attacking left/right, vertical when attaching up/down.")]
    	public bool useDirectionalHitbox;
    	public Vector2 horizontalHitboxSize = new Vector2(1f, 0.25f);
    	public Vector2 verticalHitboxSize = new Vector2(0.25f, 1f);
    	 	    
    	[Header("Ranged (Bow only)")]
    	public Arrow arrowPrefab;
    	[Tooltip("Base damage of one arrow before multiplier")]
    	public int projectileDamage = 2;
    	public float projectileSpeed = 10f;
    	[Tooltip("Seconds until arrow that hits nothing disappears")]
    	public float projectileLifetime = 2f;
    	public float projectileSpawnDistance = 0.5f;
    }
    
    [SerializeField] private WeaponConfig[] weapons;
    
    [Header("Stats")]
    [Tooltip("Bonus damage per point of strength, as a percentage. 0.05 = +5% damage per point, so a strength of 5 gives +25%.")]
    [SerializeField] private float strengthDamageScale = 0.05f;
    
    private bool isAttacking;
    private PlayerHealth playerHealth;
    private WeaponConfig currentWeapon;
    
    private PlayerData Data
    {
    	get
    	{
    		if (PlayerDataManager.Instance == null)
    		{
    			Debug.LogError("PlayerCombat needs a 'PlayerDataManager in the scene but none was found.");
    			return null;
    		}
    		
    		return PlayerDataManager.Instance.Data;
    	}
    }
    
    public bool IsAttacking => isAttacking;
    public WeaponType CurrentWeaponType => currentWeapon != null ? currentWeapon.type : default;
    
    private void Awake()
    {
    	foreach (WeaponConfig weapon in weapons)
    	{
    		if (weapon.hitbox == null)
    			continue;
    			
    		Collider2D collider = weapon.hitbox.GetComponent<Collider2D>();
    		
    		if (collider != null)
    			collider.enabled = false;
    			
    		if (weapon.hitEffectPrefab != null)
    			weapon.hitbox.hitEffectPrefab = weapon.hitEffectPrefab;
    	}
    	
    	if (weapons.Length > 0)
    		currentWeapon = weapons[0];

    	playerHealth = GetComponent<PlayerHealth>();
    }
    
    private void Update()
    {
    	if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
    		TryAttack();
    }
    
    public bool TryAttack()
    {
    	if (isAttacking)
    		return false;
    		
    	if (currentWeapon == null)
    		return false;
    		
    	if (playerHealth != null && playerHealth.IsDead)
    	{
    		Debug.Log("Cannot attack when dead.");
    		return false;
    	}
    	
    	StartCoroutine(Attack(currentWeapon));
    	return true;
    }
    
    private IEnumerator Attack(WeaponConfig weapon)
    {
    	isAttacking = true;
    	
    	if (weapon.type == WeaponType.Bow)
    	{
    		FireArrow(weapon);
    		
    		yield return new WaitForSeconds(weapon.attackDuration + weapon.attackCooldown);
    		
    		isAttacking = false;
    		yield break;
    	}
    	
    	Collider2D hitboxCollider = weapon.hitbox.GetComponent<Collider2D>();
    	
    	PositionWeaponTowardMouse(weapon);
    	
    	weapon.hitbox.DamageMultiplier = CalculateStrengthDamageMultiplier();
    	
    	weapon.hitbox.ResetSwing();
    	hitboxCollider.enabled = true;
    	
    	weapon.hitbox.PlaySwingEffect();
    	
    	yield return new WaitForSeconds(weapon.attackDuration);
    	
    	hitboxCollider.enabled = false;
    	
    	yield return new WaitForSeconds(weapon.attackCooldown);
    	
    	isAttacking = false;
    }
    
    private void FireArrow(WeaponConfig weapon)
    {
    	Vector2 direction = GetAimDirection();
    	Vector2 spawnPosition = (Vector2)transform.position + direction * weapon.projectileSpawnDistance;
    	
    	int damage = Math.Max(1, Mathf.RoundToInt(weapon.projectileDamage * CalculateStrengthDamageMultiplier()));
    	
    	Arrow arrow;
    	if (weapon.arrowPrefab != null)
    		arrow = Instantiate(weapon.arrowPrefab, spawnPosition, Quaternion.identity);
    		
    	else
    	{
    		SpriteRenderer playerSprite = GetComponentInChildren<SpriteRenderer>();
    		int layerId = playerSprite != null ? playerSprite.sortingLayerID : 0;
    		int order = (playerSprite != null ? playerSprite.sortingOrder : 0) + 1;
    		arrow = Arrow.CreatePlaceholder(spawnPosition, layerId, order);
    	}
    	
    	arrow.Initialize(direction, damage, weapon.projectileSpeed, weapon.projectileLifetime, transform);
    }
    
    private Vector2 GetAimDirection()
    {
    	if (Camera.main == null || Mouse.current == null)
    		return Vector2.right;
    		
    	Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    	Vector2 direction = mouseWorldPos - (Vector2)transform.position;
    	
    	return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
    }
    
    private void PositionWeaponTowardMouse(WeaponConfig weapon)
    {
    	if (Camera.main == null || Mouse.current == null)
    		return;
    		
    	Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    	Vector2 direction = (mouseWorldPos - (Vector2)transform.position).normalized;
    	
    	weapon.hitbox.transform.localPosition = direction * weapon.attackDistance;
    	
    	if (weapon.useDirectionalHitbox)
    		ApplyDirectionalHitboxShape(weapon, direction);
    }
    
    private void ApplyDirectionalHitboxShape(WeaponConfig weapon, Vector2 direction)
    {
    	BoxCollider2D collider = weapon.hitbox.GetComponent<BoxCollider2D>();
    	
    	if (collider == null)
    		return;
    		
    	bool isHorizontal = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
    	collider.size = isHorizontal ? weapon.horizontalHitboxSize : weapon.verticalHitboxSize;
    }
    
    private float CalculateStrengthDamageMultiplier()
    {
    	PlayerData data = Data;
    	
    	if (data == null)
    		return 1f;
    		
    	return 1f + (data.strength * strengthDamageScale);
    }
    
    public void EquipSword(InputAction.CallbackContext context)
    {
    	if (!context.performed)
    		return;
    		
    	SwitchWeapon(WeaponType.Sword);
    }
    
    public void EquipSpear(InputAction.CallbackContext context)
    {
    	if (!context.performed)
    		return;
    		
    	SwitchWeapon(WeaponType.Spear);
    }
    
    public void EquipBow(InputAction.CallbackContext context)
    {
    	if (!context.performed)
    		return;
    		
    	SwitchWeapon(WeaponType.Bow);
    }
    
    public bool SwitchWeapon(WeaponType type)
    {
    	if (isAttacking)
    		return false;
    		
    	WeaponConfig target = Array.Find(weapons, w => w.type == type);
    	
    	if (target == null)
    		return false;
    		
    	currentWeapon = target;
    	
    	return true;
    }
}
