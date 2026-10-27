using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class PlayerMovementTests : InputTestFixture
{
    private GameObject playerDataManagerObject;
    private PlayerDataManager playerDataManager;

    private GameObject player;
    private Rigidbody2D rb;
    private PlayerMovement movement;
    private PlayerHealth health;
    private Keyboard keyboard;
    private InputAction moveAction;
    private InputAction dodgeAction;
    
    private InputAction CreateMoveAction()
    {
    	InputAction moveAction = new InputAction(
    		type: InputActionType.Value,
    		expectedControlType: "Vector2"
    	);
    	
    	moveAction.AddCompositeBinding("2DVector")
    		.With("Up", "<Keyboard>/w")
    		.With("Down", "<Keyboard>/s")
    		.With("Left", "<Keyboard>/a")
    		.With("Right", "<Keyboard>/d");
    		
    	moveAction.performed += movement.Move;
    	moveAction.canceled += movement.Move;
    	
    	moveAction.Enable();
    	
    	return moveAction;
    }
    
    private InputAction CreateDodgeAction()
    {
    	InputAction dodgeAction = new InputAction(
    		type: InputActionType.Button,
    		binding: "<Keyboard>/space");
    		
    	dodgeAction.performed += movement.Dodge;
    	
    	dodgeAction.Enable();
    	
    	return dodgeAction;
    }
    
    [SetUp]
    public override void Setup()
    {
    	base.Setup();
    	
    	keyboard = InputSystem.AddDevice<Keyboard>();
    	
    	playerDataManagerObject = new GameObject("TestPlayerDataManager");
    	playerDataManager = playerDataManagerObject.AddComponent<PlayerDataManager>();
    	
    	player = new GameObject("Player");
    	player.SetActive(false);
    	
    	rb = player.AddComponent<Rigidbody2D>();
    	player.AddComponent<Animator>();
    	health = player.AddComponent<PlayerHealth>();
    	movement = player.AddComponent<PlayerMovement>();
    	
    	SetPrivateField(health, "disableWhileDead", new Behaviour[0]);
    	
    	player.SetActive(true);
    	
    	moveAction = CreateMoveAction();
    	dodgeAction = CreateDodgeAction();
    }
    
    [TearDown]
    public override void TearDown()
    {
    	if (moveAction != null)
    	{
    		moveAction.Disable();
    		moveAction.Dispose();
    	}
    	
    	if (dodgeAction != null)
    	{
    		dodgeAction.Disable();
    		dodgeAction.Dispose();
    	}
    	if (player != null)
    		Object.Destroy(player);
    		
    	base.TearDown();
    	
    	if (playerDataManagerObject != null)
    		Object.DestroyImmediate(playerDataManagerObject);
    }
    
    private static void SetPrivateField(object target, string fieldName, object value)
    {
    	FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
    	Assert.NotNull(field, $"Expected a private field name '{fieldName}' on {target.GetType().Name}");
    	field.SetValue(target, value);
    }
    
    private static T GetPrivateField<T>(object target, string fieldName)
    {
    	FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
    	Assert.NotNull(field, $"Expected a private field name '{fieldName}' on {target.GetType().Name}");
    	return (T)field.GetValue(target);
    }
    
    [UnityTest]
    public IEnumerator PlayerMovesRight_WhenDPressed()
    {
    	yield return null;
    	
    	Press(keyboard.dKey);
    	
    	yield return null;
    	
    	Assert.Greater(rb.linearVelocity.x, 0f);
    	
    	Release(keyboard.dKey);
    }
    
    [UnityTest]
    public IEnumerator PlayerMovesRight_WhenAPressed()
    {
    	yield return null;
    	
    	Press(keyboard.aKey);
    	
    	yield return null;
    	
    	Assert.Less(rb.linearVelocity.x, 0f);
    	
    	Release(keyboard.aKey);
    }
    
    [UnityTest]
    public IEnumerator PlayerMovesRight_WhenWPressed()
    {
    	yield return null;
    	
    	Press(keyboard.wKey);
    	
    	yield return null;
    	
    	Assert.Greater(rb.linearVelocity.y, 0f);
    	
    	Release(keyboard.wKey);
    }
    
    [UnityTest]
    public IEnumerator PlayerMovesRight_WhenSPressed()
    {
    	yield return null;
    	
    	Press(keyboard.sKey);
    	
    	yield return null;
    	
    	Assert.Less(rb.linearVelocity.y, 0f);
    	
    	Release(keyboard.sKey);
    }
    
    // ========== Tests for dodge mechanic ==============
    
    [UnityTest]
    public IEnumerator Dodge_AppliesVelocityInHeldDirection()
    {
    	yield return null;
    	
    	Press(keyboard.aKey);
    	yield return null;
    	
    	bool started = movement.TryDodge();
    	
    	float dodgeSpeed = GetPrivateField<float>(movement, "dodgeSpeed");
    	Assert.IsTrue(started);
    	Assert.AreEqual(Vector2.left * dodgeSpeed, rb.linearVelocity);
    	
    	Release(keyboard.aKey);
    }
    
    [UnityTest]
    public IEnumerator Dodge_DefaultsToRight_NoInputHeld()
    {
    	yield return null;
    	
    	bool started = movement.TryDodge();
    	
    	float dodgeSpeed = GetPrivateField<float>(movement, "dodgeSpeed");
    	Assert.IsTrue(started);
    	Assert.AreEqual(Vector2.right * dodgeSpeed, rb.linearVelocity);
    }
    
    [UnityTest]
    public IEnumerator Dodge_CannotBeTriggered_OnCooldown()
    {
    	yield return null;
    	
    	bool first = movement.TryDodge();
    	Assert.IsTrue(first);
    	
    	bool second = movement.TryDodge();
    	Assert.IsFalse(second);
    }
    
    [UnityTest]
    public IEnumerator Dodge_Available_AfterCooldown()
    {
    	yield return null;
    	
    	movement.TryDodge();
    	
    	float dodgeCooldown = GetPrivateField<float>(movement, "dodgeCooldown");
    	yield return new WaitForSeconds(dodgeCooldown + 0.1f);
    	
    	bool secondDodge = movement.TryDodge();
    	Assert.IsTrue(secondDodge);
    }
    
    [UnityTest]
    public IEnumerator Dodge_GrantsInvincibility()
    {
    	yield return null;
    	
    	int healthBeforeDamage = playerDataManager.Data.currentHealth;
    	
    	movement.TryDodge();
    	health.TakeDamage(10);
    	
    	Assert.AreEqual(healthBeforeDamage, playerDataManager.Data.currentHealth);
    }
    
    [UnityTest]
    public IEnumerator Dodge_InvincibilityEndsWIthDodge()
    {
    	yield return null;
    	
    	movement.TryDodge();
    	
    	float dodgeDuration = GetPrivateField<float>(movement, "dodgeDuration");
    	yield return new WaitForSeconds(dodgeDuration + 0.05f);
    	
    	int healthBeforeDamage = playerDataManager.Data.currentHealth;
    	health.TakeDamage(10);
    	
    	Assert.AreEqual(healthBeforeDamage - 10, playerDataManager.Data.currentHealth);
    }
    
    [UnityTest]
    public IEnumerator Dodge_VelocityClears_AfterDodgeEnds()
    {
    	yield return null;
    	
    	movement.TryDodge();
    	Assert.AreNotEqual(Vector2.zero, rb.linearVelocity);
    	
    	float dodgeDuration = GetPrivateField<float>(movement, "dodgeDuration");
    	yield return new WaitForSeconds(dodgeDuration + 0.05f);
    	
    	Assert.AreEqual(Vector2.zero, rb.linearVelocity);
    }
    
    [UnityTest]
    public IEnumerator Movement_ResumesAfterDodge()
    {
    	yield return null;
 
    	movement.TryDodge();
 
    	float dodgeDuration = GetPrivateField<float>(movement, "dodgeDuration");
    	yield return new WaitForSeconds(dodgeDuration + 0.05f);
 
    	Press(keyboard.dKey);
    	yield return null;
 
    	Assert.Greater(rb.linearVelocity.x, 0f);
 
    	Release(keyboard.dKey);
    }
    
    [UnityTest]
    public IEnumerator Dodge_BlockedWhileDead()
    {
    	yield return null;
 
    	health.TakeDamage(9999);
    	yield return null;
 
    	Assert.IsTrue(health.IsDead);
 
    	bool started = movement.TryDodge();
    	Assert.IsFalse(started);
    }
    
    [UnityTest]
    public IEnumerator PlayerDodges_WhenSpacePressed()
    {
    	yield return null;
 
    	Press(keyboard.spaceKey);
    	yield return null;
 
    	Assert.AreNotEqual(Vector2.zero, rb.linearVelocity);
 
    	Release(keyboard.spaceKey);
    }
}
