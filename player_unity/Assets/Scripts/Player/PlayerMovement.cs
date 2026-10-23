using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5f;
	
	[Header("Dodge")]
	[SerializeField] private float dodgeSpeed = 12f;
	[SerializeField] private float dodgeDuration = 0.2f;
	[SerializeField] private float dodgeCooldown = 0.5f;
	[SerializeField] private int dodgeStaminaCost = 20;
	
	private Rigidbody2D rb;
	private Vector2 moveInput;
	private Animator animator;
	private PlayerHealth playerHealth;
	
	private bool isDodging;
	private float dodgeCooldownRemaining;
	
	private PlayerData Data
	{
		get
		{
			if (PlayerDataManager.Instance == null)
			{
				Debug.LogError("PlayerMovement needs a PlayerDataManager in the scene.");
				return null;
			}
			
			return PlayerDataManager.Instance.Data;
		}
	}
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
    	if (dodgeCooldownRemaining > 0f)
    		dodgeCooldownRemaining -= Time.deltaTime;
    		
    	if (isDodging)
    		return;
    		
        rb.linearVelocity = moveInput * moveSpeed;
    }
    
    public void Move(InputAction.CallbackContext context)
    {
    	animator.SetBool("isWalking", true);
    	
    	if (context.canceled) 
    	{
    		animator.SetBool("isWalking", false);
    		animator.SetFloat("LastInputX", moveInput.x);
    		animator.SetFloat("LastInputY", moveInput.y);
    	}
    	moveInput = context.ReadValue<Vector2>();
    	animator.SetFloat("InputX", moveInput.x);
    	animator.SetFloat("InputY", moveInput.y);
    }
    
    public void Dodge(InputAction.CallbackContext context)
    {
    	if (!context.performed)
    		return;
    		
    	TryDodge();
    }
    
    public bool TryDodge()
    {
    	if (isDodging || dodgeCooldownRemaining > 0f)
    		return false;
    		
    	if (playerHealth != null && playerHealth.IsDead)
    		return false;
    		
    	PlayerData data = Data;
    	if (data == null || data.currentStamina < dodgeStaminaCost)
    		return false;
    		
    	//data.currentStamina -= dodgeStaminaCost;
    	
    	Vector2 direction = moveInput.sqrMagnitude > 0.0001f ? moveInput.normalized : Vector2.right;
    	StartCoroutine(DodgeRoutine(direction));
    	return true;
    }
    
    private IEnumerator DodgeRoutine(Vector2 direction)
    {
    	isDodging = true;
    	dodgeCooldownRemaining = dodgeCooldown;
    	
    	if (playerHealth != null)
    		playerHealth.SetDodgeInvincible(dodgeDuration);
    	else
    		Debug.LogError("PlayerMovement: no PlayerHealth found - dodge can't grant invincibility.");
    		
    	//animator.SetTrigger("Dodge");
    	
    	rb.linearVelocity = direction * dodgeSpeed;
    	
    	yield return new WaitForSeconds(dodgeDuration);
    	
    	rb.linearVelocity = Vector2.zero;
    	isDodging = false;
    }
}
