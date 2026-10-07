using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private WeaponHitbox swordHitbox;
    [SerializeField] private float attackDuration = 0.2f;
    [SerializeField] private float attackCooldown = 0.4f;
    [SerializeField] private float swordDistance = 0.50f;
    
    private bool isAttacking;
    private Collider2D hitboxCollider;
    
    private void Awake()
    {
    	hitboxCollider = swordHitbox.GetComponent<Collider2D>();
    	hitboxCollider.enabled = false;
    }
    
    private void Update()
    {
    	if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && !isAttacking)
    		StartCoroutine(Attack());
    }
    
    private IEnumerator Attack()
    {
    	isAttacking = true;
    	
    	PositionSwordTowardMouse();
    	
    	swordHitbox.ResetSwing();
    	hitboxCollider.enabled = true;
    	
    	yield return new WaitForSeconds(attackDuration);
    	
    	hitboxCollider.enabled = false;
    	
    	yield return new WaitForSeconds(attackCooldown);
    	
    	isAttacking = false;
    }
    
    private void PositionSwordTowardMouse()
    {
    	if (Camera.main == null || Mouse.current == null)
    		return;
    		
    	Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    	Vector2 direction = (mouseWorldPos - (Vector2)transform.position).normalized;
    	
    	swordHitbox.transform.localPosition = direction * swordDistance;
    }
}
