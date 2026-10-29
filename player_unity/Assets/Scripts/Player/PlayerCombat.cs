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
    }
    
    
    [SerializeField] private WeaponConfig[] weapons;
    
    private bool isAttacking;
    private PlayerHealth playerHealth;
    private WeaponConfig currentWeapon;
    
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
    		return false;
    		
    	StartCoroutine(Attack(currentWeapon));
    	return true;
    }
    
    private IEnumerator Attack(WeaponConfig weapon)
    {
    	isAttacking = true;
    	
    	Collider2D hitboxCollider = weapon.hitbox.GetComponent<Collider2D>();
    	
    	PositionWeaponTowardMouse(weapon);
    	
    	weapon.hitbox.ResetSwing();
    	hitboxCollider.enabled = true;
    	
    	weapon.hitbox.PlaySwingEffect();
    	
    	yield return new WaitForSeconds(weapon.attackDuration);
    	
    	hitboxCollider.enabled = false;
    	
    	yield return new WaitForSeconds(weapon.attackCooldown);
    	
    	isAttacking = false;
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
