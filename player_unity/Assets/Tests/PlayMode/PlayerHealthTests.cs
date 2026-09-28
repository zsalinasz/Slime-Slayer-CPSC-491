using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHealthTests
{
    private GameObject player;
    private PlayerHealth health;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    private int maxHealth;

    [SetUp]
    public void SetUp()
    {
        player = new GameObject("Player");

        rb = player.AddComponent<Rigidbody2D>();

        GameObject spriteObject = new GameObject("Sprite");
        spriteObject.transform.SetParent(player.transform);
        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();

        health = player.AddComponent<PlayerHealth>();

        SetPrivateField("respawnDelay", 0.1f);
        SetPrivateField("invincibilityTime", 0.1f);
        SetPrivateField("flashInterval", 0.02f);
        SetPrivateField("knockbackDuration", 0.05f);
        SetPrivateField("disableWhileDead", new Behaviour[0]);

        health.OnHealthChanged.AddListener(
            (current, max) =>
            {
                currentHealth = current;
                maxHealth = max;
            }
        );
    }

    [TearDown]
    public void TearDown()
    {
        if (player != null)
        	Object.Destroy(player);
    }

    private void SetPrivateField(string fieldName, object value)
    {
        FieldInfo field = typeof(PlayerHealth).GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        field.SetValue(health, value);
    }
    
    [UnityTest]
    public IEnumerator PlayerStartsAtMaximumHealth()
    {
    	yield return null;
    	
    	Assert.AreEqual(3, currentHealth);
    	Assert.AreEqual(3, maxHealth);
    }
    
    [UnityTest]
    public IEnumerator TakingDamage_ReducesHealth()
    {
    	yield return null;
    	
    	health.TakeDamage(1);
    	
    	Assert.AreEqual(2, currentHealth);
    }
    
    [UnityTest]
    public IEnumerator HealthDoesNotGoBelowZero()
    {
    	yield return null;
    	
    	health.TakeDamage(50);
    	
    	Assert.AreEqual(0, currentHealth);
    }
    
    [UnityTest]
    public IEnumerator Healing_IncreasesHealth()
    {
    	yield return null;
    	
    	health.TakeDamage(1);
    	
    	Assert.AreEqual(2, currentHealth);
    	
    	health.Heal(1);
    	
    	Assert.AreEqual(3, currentHealth);
    }
    
    [UnityTest]
    public IEnumerator Healing_DoesNotExceedMaximumHealth()
    {
    	yield return null;
    	
    	health.Heal(50);
    	
    	Assert.AreEqual(maxHealth, currentHealth);
    }
    
    [UnityTest]
    public IEnumerator Invincibility_PreventsImmediateSecondHit()
    {
    	yield return null;
    	
    	health.TakeDamage(1);
    	
    	Assert.AreEqual(2, currentHealth);
    	
    	health.TakeDamage(1);
    	
    	Assert.AreEqual(2, currentHealth);
    }
    
    [UnityTest]
    public IEnumerator PlayerTakesDamageAfterInvincibilityEnds()
    {
    	yield return null;
    	
    	health.TakeDamage(1);
    	
    	Assert.AreEqual(2, currentHealth);
    	
    	yield return new WaitForSeconds(0.15f);
    	
    	health.TakeDamage(1);
    	
    	Assert.AreEqual(1, currentHealth);
    }
    
    [UnityTest]
    public IEnumerator LethalDamage_KillsPlayer()
    {
    	yield return null;
    	
    	health.TakeDamage(3);
    	
    	Assert.IsTrue(health.IsDead);
    }
    
    [UnityTest]
    public IEnumerator Death_InvokesOnDiedEvent()
    {
    	yield return null;
    	
    	bool diedEventCalled = false;
    	
    	health.OnDied.AddListener(() => 
    	{
    		diedEventCalled = true;
    	});
    	
    	health.TakeDamage(3);
    	
    	Assert.IsTrue(diedEventCalled);
    }
    
    [UnityTest]
    public IEnumerator DeathEvent_InvokedOnce()
    {
    	yield return null;
    	
    	int deathCount = 0;
    	
    	health.OnDied.AddListener(() => 
    	{
    		deathCount++;
    	});
    	
    	health.TakeDamage(3);
    	health.TakeDamage(3);
    	
    	Assert.AreEqual(1, deathCount);
    }
    
    [UnityTest]
    public IEnumerator DeadPlayer_RespawnsAtPoint()
    {
    	yield return null;
    	
    	GameObject respawn = new GameObject("RespawnPoint");
    	respawn.transform.position = new Vector3(0.41f, -0.14f, 0f);
    	
    	health.SetRespawnPoint(respawn.transform);
    	
    	health.TakeDamage(3);
    	
    	Assert.IsTrue(health.IsDead);
    	
    	yield return new WaitForSeconds(0.15f);
    	
    	Assert.AreEqual(
    		(Vector2)respawn.transform.position,
    		rb.position
    	);
    	
    	Object.Destroy(respawn);
    }
    
    [UnityTest]
    public IEnumerator Respawn_RestoreMaxHealth()
    {
    	yield return null;
    	
    	health.TakeDamage(3);
    	
    	Assert.AreEqual(0, currentHealth);
    	
    	yield return new WaitForSeconds(0.15f);
    	
    	Assert.AreEqual(maxHealth, currentHealth);
    }
    
    [UnityTest]
    public IEnumerator Respawn_ClearDeadState()
    {
    	yield return null;
    	
    	health.TakeDamage(3);
    	
    	Assert.IsTrue(health.IsDead);
    	
    	yield return new WaitForSeconds(0.15f);
    }
    
    [UnityTest]
    public IEnumerator Respawn_InvokeOnRespawnedEvent()
    {
    	yield return null;
    	
    	bool respawnEventCalled = false;
    	
    	health.OnRespawned.AddListener(() => 
    	{
    		respawnEventCalled = true;
    	});
    	
    	health.TakeDamage(3);
    	
    	yield return new WaitForSeconds(0.15f);
    	
    	Assert.IsTrue(respawnEventCalled);
    }
    
    [UnityTest]
    public IEnumerator DamageFromEnemy_AppliesKnockback()
    {
    	yield return null;
    	
    	player.transform.position = Vector2.zero;
    	
    	Vector2 sourcePosition = new Vector2(-1f, 0f);
    	
    	health.TakeDamage(1, sourcePosition);
    	
    	Assert.Greater(rb.linearVelocity.x, 0f);
    }
    
    
    [UnityTest]
    public IEnumerator Knockback_StopsAfterDuration()
    {
    	yield return null;
    	
    	player.transform.position = Vector2.zero;
    	
    	health.TakeDamage(
    		1,
    		new Vector2(-1f, 0f)
    	);
    	
    	Assert.Greater(rb.linearVelocity.x, 0f);
    	
    	yield return new WaitForSeconds(0.1f);
    	
    	Assert.AreEqual(
    		Vector2.zero,
    		rb.linearVelocity
    	);
    }
    
    public class TestControlBehaviour : MonoBehaviour
    {
    }
    
    [UnityTest]
    public IEnumerator Knockback_DisablesAndRestoresControls()
    {
    	yield return null;
    	
    	TestControlBehaviour controls = 
    		player.AddComponent<TestControlBehaviour>();
    		
    	SetPrivateField(
    		"disableWhileDead",
    		new Behaviour[] { controls }
    	);
    	
    	health.TakeDamage(
    		1,
    		new Vector2(-1f, 0f)
    	);
    	
    	Assert.IsFalse(controls.enabled);
    	
    	yield return new WaitForSeconds(0.1f);
    	
    	Assert.IsTrue(controls.enabled);
    }
    
    [UnityTest]
    public IEnumerator Death_DisablesControls()
    {
        yield return null;

        TestControlBehaviour controls =
        	player.AddComponent<TestControlBehaviour>();

        SetPrivateField(
        	"disableWhileDead",
        	new Behaviour[] { controls }
    	);

        health.TakeDamage(3);

        Assert.IsFalse(controls.enabled);
    }
    
    [UnityTest]
    public IEnumerator Respawn_RestoresControls()
    {
        yield return null;

        TestControlBehaviour controls =
        	player.AddComponent<TestControlBehaviour>();

        SetPrivateField(
        	"disableWhileDead",
        	new Behaviour[] { controls }
        );

        health.TakeDamage(3);

        Assert.IsFalse(controls.enabled);

        yield return new WaitForSeconds(0.15f);

        Assert.IsTrue(controls.enabled);
    }
    
    [UnityTest]
    public IEnumerator EnvironmentalDamage_DoesNotApplyKnockback()
    {
        yield return null;

        health.TakeDamage(1);

        yield return null;

        Assert.AreEqual(
        	0f,
        	rb.linearVelocity.magnitude,
        	0.01f
        );
    }
    
    [UnityTest]
    public IEnumerator Respawn_ProvidesShortInvincibility()
    {
        yield return null;

        health.TakeDamage(3);

        yield return new WaitForSeconds(0.12f);

        Assert.IsFalse(health.IsDead);
        Assert.AreEqual(maxHealth, currentHealth);

        health.TakeDamage(1);

        Assert.AreEqual(maxHealth, currentHealth);
    }
}
