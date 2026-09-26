using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public class PlayerMovementTests : InputTestFixture
{
    private GameObject player;
    private Rigidbody2D rb;
    private PlayerMovement movement;
    private Keyboard keyboard;
    private InputAction moveAction;
    
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
    
    [SetUp]
    public override void Setup()
    {
    	base.Setup();
    	
    	keyboard = InputSystem.AddDevice<Keyboard>();
    	
    	player = new GameObject("Player");
    	
    	rb = player.AddComponent<Rigidbody2D>();
    	player.AddComponent<Animator>();
    	movement = player.AddComponent<PlayerMovement>();
    	
    	moveAction = CreateMoveAction();
    }
    
    [TearDown]
    public override void TearDown()
    {
    	Object.Destroy(player);
    	base.TearDown();
    	
    	if (moveAction != null)
    	{
    		moveAction.Disable();
    		moveAction.Dispose();
    	}
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
}
