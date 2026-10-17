using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SwordCombatTests
{
    private GameObject playerObject;
    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;
    
    private GameObject hitboxObject;
    private WeaponHitbox weaponHitbox;
    
    [SetUp]
    public void SetUp()
    {
    	playerObject = new GameObject("TestPlayer");
    	playerObject.SetActive(false);
    	
    	playerObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    	playerHealth = playerObject.AddComponent<PlayerHealth>();
    	playerCombat = playerObject.AddComponent<PlayerCombat>();
    	
    	SetPrivateField(playerHealth, "disableWhileDead", new Behaviour[0]);
    	
    	hitboxObject = new GameObject("TestSwordHitbox");
    	hitboxObject.transform.SetParent(playerObject.transform);
    	hitboxObject.transform.localPosition = Vector2.zero;
    	
    	BoxCollider2D hitboxCollider = hitboxObject.AddComponent<BoxCollider2D>();
    	hitboxCollider.isTrigger = true;
    	hitboxCollider.size = new Vector2(0.5f, 0.5f);
    	
    	weaponHitbox = hitboxObject.AddComponent<WeaponHitbox>();
        GameObject dummyPrefab = new GameObject("TestDummyPrefab");
        weaponHitbox.hitEffectPrefab = dummyPrefab;
    	
    	SetPrivateField(playerCombat, "swordHitbox", weaponHitbox);
    	
    	playerObject.SetActive(true);
    }
    
    [TearDown]
    public void TearDown()
    {
    	if (playerObject != null)
    		Object.Destroy(playerObject);
    }
    
    private static void SetPrivateField(object target, string fieldName, object value)
    {
    	FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
    	Assert.NotNull(field, $"Expected a private field named '{fieldName}' on {target.GetType().Name}");
    	field.SetValue(target, value);
    }
    
    private static T GetPrivateField<T>(object target, string fieldName)
    {
    	FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
    	Assert.NotNull(field, $"Expected a private field named '{fieldName}' on {target.GetType().Name}");
    	return (T)field.GetValue(target);
    }
    
    private TestDamageable CreateEnemy(Vector2 position)
    {
    	GameObject enemy = new GameObject("TestEnemy");
    	enemy.transform.position = position;
    	
    	BoxCollider2D collider = enemy.AddComponent<BoxCollider2D>();
    	collider.isTrigger = true;
    	collider.size = new Vector2(0.5f, 0.5f);
    	
    	return enemy.AddComponent<TestDamageable>();
    }
    
    [Test]
    public void SwordAttack_CanBeTriggered()
    {
    	bool started = playerCombat.TryAttack();
    	
    	Assert.IsTrue(started, "TryAttack() should report that attack started.");
    	Assert.IsTrue(playerCombat.IsAttacking, "PlayerCombat should be in attacking state after TryAttack().");
    }
    
    [UnityTest]
    public IEnumerator EnemyInsideAttackRange_TakesDamage()
    {
    	TestDamageable enemy = CreateEnemy(playerObject.transform.position);
    	
    	playerCombat.TryAttack();
    	
    	yield return new WaitForFixedUpdate();
    	yield return new WaitForFixedUpdate();
    	
    	Assert.AreEqual(1, enemy.HitCount);
    	Assert.AreEqual(weaponHitbox.Damage, enemy.LastDamageReceived);
    }
    
    [UnityTest]
    public IEnumerator EnemyOutsideAttackRange_TakesNoDamage()
    {
    	TestDamageable enemy = CreateEnemy(playerObject.transform.position + new Vector3(10f, 10f, 0f));
    	
    	playerCombat.TryAttack();
    	
    	yield return new WaitForFixedUpdate();
    	yield return new WaitForFixedUpdate();
    	
    	Assert.AreEqual(0, enemy.HitCount);
    }
    
    [UnityTest]
    public IEnumerator SwordDamage_MatchesConfigValue()
    {
    	const int expectedDamage = 5;
    	SetPrivateField(weaponHitbox, "damage", expectedDamage);
    	
    	TestDamageable enemy = CreateEnemy(playerObject.transform.position);
    	
    	playerCombat.TryAttack();
    	
    	yield return new WaitForFixedUpdate();
    	yield return new WaitForFixedUpdate();
    	
    	Assert.AreEqual(expectedDamage, enemy.LastDamageReceived);
    }
    
    [UnityTest]
    public IEnumerator DeadPlayer_CannotAttack()
    {
    	yield return null;
    	
    	playerHealth.TakeDamage(999);
    	
    	yield return null;
    	
    	Assert.IsTrue(playerHealth.IsDead);
    	
    	bool started = playerCombat.TryAttack();
    	
    	Assert.IsFalse(started);
    	Assert.IsFalse(playerCombat.IsAttacking);
    }
    
    [UnityTest]
    public IEnumerator OneSwing_DoesNotDuplicateDamage()
    {
    	TestDamageable enemy = CreateEnemy(playerObject.transform.position);
    	Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
    	
    	playerCombat.TryAttack();
    	
    	MethodInfo onTriggerEnter = typeof(WeaponHitbox).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
    	Assert.NotNull(onTriggerEnter);
    	
    	onTriggerEnter.Invoke(weaponHitbox, new object[] { enemyCollider });
    	onTriggerEnter.Invoke(weaponHitbox, new object[] { enemyCollider });
    	
    	yield return null;
    	
    	Assert.AreEqual(1, enemy.HitCount);
    }
    
    [UnityTest]
    public IEnumerator AttackState_ReturnsToValidStateAfterFinishing()
    {
    	float attackDuration = GetPrivateField<float>(playerCombat, "attackDuration");
    	float attackCooldown = GetPrivateField<float>(playerCombat, "attackCooldown");
    	
    	playerCombat.TryAttack();
    	Assert.IsTrue(playerCombat.IsAttacking);
    	
    	yield return new WaitForSeconds(attackDuration + attackCooldown + 0.1f);
    	
    	Assert.IsFalse(playerCombat.IsAttacking);
    	
    	bool startedAgain = playerCombat.TryAttack();
    	Assert.IsTrue(startedAgain);
    }
}
