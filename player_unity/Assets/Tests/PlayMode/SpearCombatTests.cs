using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class SpearCombatTests : InputTestFixture
{
    private GameObject playerDataManagerObject;
    private PlayerDataManager playerDataManager;

    private GameObject playerObject;
    private PlayerHealth playerHealth;
    private PlayerCombat playerCombat;

    private WeaponHitbox swordHitbox;
    private WeaponHitbox spearHitbox;
    private BoxCollider2D swordCollider;
    private BoxCollider2D spearCollider;

    private PlayerCombat.WeaponConfig swordConfig;
    private PlayerCombat.WeaponConfig spearConfig;

    private GameObject dummyHitEffectPrefab;

    private Camera aimCamera;
    private Mouse mouse;
    private Vector2 lastAimScreenPoint;

    [SetUp]
    public override void Setup()
    {
    	base.Setup();

    	mouse = null;

    	playerDataManagerObject = new GameObject("TestPlayerDataManager");
    	playerDataManager = playerDataManagerObject.AddComponent<PlayerDataManager>();

    	dummyHitEffectPrefab = new GameObject("DummyHitEffectPrefab");

    	playerObject = new GameObject("TestPlayer");
    	playerObject.SetActive(false);

    	playerObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    	playerHealth = playerObject.AddComponent<PlayerHealth>();
    	playerCombat = playerObject.AddComponent<PlayerCombat>();

    	SetPrivateField(playerHealth, "disableWhileDead", new Behaviour[0]);

    	swordHitbox = CreateHitbox("TestSwordHitbox", out swordCollider);
    	spearHitbox = CreateHitbox("TestSpearHitbox", out spearCollider);

    	swordConfig = new PlayerCombat.WeaponConfig
    	{
    		type = WeaponType.Sword,
    		hitbox = swordHitbox,
    		attackDuration = 0.2f,
    		attackCooldown = 0.4f,
    		attackDistance = 0.5f,
    		hitEffectPrefab = dummyHitEffectPrefab
    	};

    	spearConfig = new PlayerCombat.WeaponConfig
    	{
    		type = WeaponType.Spear,
    		hitbox = spearHitbox,
    		attackDuration = 0.2f,
    		attackCooldown = 0.4f,
    		attackDistance = 0.75f,
    		hitEffectPrefab = dummyHitEffectPrefab,
    		useDirectionalHitbox = true,
    		horizontalHitboxSize = new Vector2(1f, 0.25f),
    		verticalHitboxSize = new Vector2(0.25f, 1f)
    	};

    	SetPrivateField(playerCombat, "weapons", new[] { swordConfig, spearConfig });

    	playerObject.SetActive(true);
    }

    [TearDown]
    public override void TearDown()
    {
    	if (playerObject != null) 
    		Object.Destroy(playerObject);
    		
    	if (dummyHitEffectPrefab != null) 
    		Object.Destroy(dummyHitEffectPrefab);

    	if (aimCamera != null) 
    		Object.DestroyImmediate(aimCamera.gameObject);

    	base.TearDown();

    	if (playerDataManagerObject != null) 
    		Object.DestroyImmediate(playerDataManagerObject);
    }

    private WeaponHitbox CreateHitbox(string objectName, out BoxCollider2D collider)
    {
    	GameObject hitboxObject = new GameObject(objectName);
    	hitboxObject.transform.SetParent(playerObject.transform);
    	hitboxObject.transform.localPosition = Vector2.zero;

    	collider = hitboxObject.AddComponent<BoxCollider2D>();
    	collider.isTrigger = true;
    	collider.size = new Vector2(0.5f, 0.5f);

    	WeaponHitbox hitbox = hitboxObject.AddComponent<WeaponHitbox>();
    	hitbox.hitEffectPrefab = dummyHitEffectPrefab;
    	return hitbox;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
    	FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
    	Assert.NotNull(field, $"Expected a private field named '{fieldName}' on {target.GetType().Name}");
    	field.SetValue(target, value);
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

    private void EquipSpear()
    {
    	Assert.IsTrue(playerCombat.SwitchWeapon(WeaponType.Spear), "Test setup: expected to be able to equip the spear.");
    }

    private void AimAt(Vector2 worldDirection)
    {
    	if (aimCamera == null)
    	{
    		GameObject cameraObject = new GameObject("TestMainCamera");
    		cameraObject.tag = "MainCamera";
    		cameraObject.transform.position = new Vector3(0f, 0f, -10f);

    		aimCamera = cameraObject.AddComponent<Camera>();
    		aimCamera.orthographic = true;
    		aimCamera.orthographicSize = 5f;
    	}

    	if (mouse != null)
    		InputSystem.RemoveDevice(mouse);
    		
    	mouse = InputSystem.AddDevice<Mouse>();

    	Vector3 target = playerObject.transform.position + (Vector3)(worldDirection.normalized * 3f);
    	lastAimScreenPoint = aimCamera.WorldToScreenPoint(target);
    	Set(mouse.position, lastAimScreenPoint);
    }

    private IEnumerator AimAtAndWaitForInput(Vector2 worldDirection)
    {
    	AimAt(worldDirection);

    	yield return null;

    	Vector2 reported = Mouse.current != null ? Mouse.current.position.ReadValue() : new Vector2(float.NaN, float.NaN);
    	Assert.AreEqual(lastAimScreenPoint.x, reported.x, 1f, "Mouse.current X doesn't match where AimAt pointed it.");
    	Assert.AreEqual(lastAimScreenPoint.y, reported.y, 1f, "Mouse.current Y doesn't match where AimAt pointed it.");
    }

    private IEnumerator WaitForAttackToFinish(float timeoutSeconds = 5f)
    {
    	float waited = 0f;
    	while (playerCombat.IsAttacking && waited < timeoutSeconds)
    	{
    		waited += Time.unscaledDeltaTime;
    		yield return null;
    	}
    }

    [Test]
    public void StartsWithFirstListedWeaponEquipped()
    {
    	Assert.AreEqual(WeaponType.Sword, playerCombat.CurrentWeaponType);
    }

    [Test]
    public void SwitchWeapon_ToSpear_Succeeds()
    {
    	bool switched = playerCombat.SwitchWeapon(WeaponType.Spear);

    	Assert.IsTrue(switched);
    	Assert.AreEqual(WeaponType.Spear, playerCombat.CurrentWeaponType);
    }

    [Test]
    public void SwitchWeapon_ToSwordAndBack_Works()
    {
    	playerCombat.SwitchWeapon(WeaponType.Spear);

    	Assert.IsTrue(playerCombat.SwitchWeapon(WeaponType.Sword));
    	Assert.AreEqual(WeaponType.Sword, playerCombat.CurrentWeaponType);
    }

    [Test]
    public void SwitchWeapon_ToUnconfiguredWeapon_IsRejected()
    {
    	bool switched = playerCombat.SwitchWeapon(WeaponType.Bow);

    	Assert.IsFalse(switched);
    	Assert.AreEqual(WeaponType.Sword, playerCombat.CurrentWeaponType, "A rejected switch should leave the current weapon alone.");
    }

    [Test]
    public void SwitchWeapon_IsRejected_WhileAttacking()
    {
    	playerCombat.TryAttack();
    	Assert.IsTrue(playerCombat.IsAttacking);

    	bool switched = playerCombat.SwitchWeapon(WeaponType.Spear);

    	Assert.IsFalse(switched);
    	Assert.AreEqual(WeaponType.Sword, playerCombat.CurrentWeaponType);
    }

    [UnityTest]
    public IEnumerator SwitchWeapon_IsRejected_DuringCooldown()
    {
    	EquipSpear();
    	playerCombat.TryAttack();

    	yield return new WaitForSeconds(spearConfig.attackDuration + 0.1f);
    	Assert.IsTrue(playerCombat.IsAttacking, "Test setup: expected to still be in cooldown.");

    	bool switched = playerCombat.SwitchWeapon(WeaponType.Sword);

    	Assert.IsFalse(switched);
    	Assert.AreEqual(WeaponType.Spear, playerCombat.CurrentWeaponType);
    }

    [UnityTest]
    public IEnumerator SwitchWeapon_Succeeds_AfterAttackFinishes()
    {
    	playerCombat.TryAttack();

    	yield return WaitForAttackToFinish();
    	Assert.IsFalse(playerCombat.IsAttacking, "Test setup: expected the attack to be over.");

    	Assert.IsTrue(playerCombat.SwitchWeapon(WeaponType.Spear));
    	Assert.AreEqual(WeaponType.Spear, playerCombat.CurrentWeaponType);
    }

    [Test]
    public void SpearAttack_CanBeTriggered()
    {
    	EquipSpear();

    	bool started = playerCombat.TryAttack();

    	Assert.IsTrue(started);
    	Assert.IsTrue(playerCombat.IsAttacking);
    }

    [UnityTest]
    public IEnumerator SpearAttack_OnlyEnablesSpearHitbox()
    {
    	EquipSpear();
    	playerCombat.TryAttack();

    	Assert.IsTrue(spearCollider.enabled, "Spear hitbox should be live during the swing.");
    	Assert.IsFalse(swordCollider.enabled, "Sword hitbox should stay off while the spear is swung.");

    	float waited = 0f;
    	while (spearCollider.enabled && waited < 5f)
    	{
    		waited += Time.unscaledDeltaTime;
    		yield return null;
    	}

    	Assert.IsFalse(spearCollider.enabled, "Spear hitbox should switch off after the swing.");
    }

    [UnityTest]
    public IEnumerator SpearAttack_EnemyInRange_TakesSpearDamage()
    {
    	SetPrivateField(swordHitbox, "damage", 1);
    	SetPrivateField(spearHitbox, "damage", 4);

    	EquipSpear();
    	TestDamageable enemy = CreateEnemy(playerObject.transform.position);

    	playerCombat.TryAttack();

    	yield return new WaitForFixedUpdate();
    	yield return new WaitForFixedUpdate();

    	Assert.AreEqual(1, enemy.HitCount);
    	Assert.AreEqual(spearHitbox.Damage, enemy.LastDamageReceived, "Enemy should receive the spear's damage.");
    	Assert.AreNotEqual(swordHitbox.Damage, enemy.LastDamageReceived, "Enemy should not receive the sword's damage.");
    }

    [UnityTest]
    public IEnumerator SpearAttack_EnemyOutOfRange_TakesNoDamage()
    {
    	EquipSpear();
    	TestDamageable enemy = CreateEnemy(playerObject.transform.position + new Vector3(10f, 10f, 0f));

    	playerCombat.TryAttack();

    	yield return new WaitForFixedUpdate();
    	yield return new WaitForFixedUpdate();

    	Assert.AreEqual(0, enemy.HitCount);
    }

    [UnityTest]
    public IEnumerator OneSpearThrust_DoesNotDuplicateDamage()
    {
    	EquipSpear();
    	TestDamageable enemy = CreateEnemy(playerObject.transform.position);
    	Collider2D enemyCollider = enemy.GetComponent<Collider2D>();

    	playerCombat.TryAttack();

    	MethodInfo onTriggerEnter = typeof(WeaponHitbox).GetMethod("OnTriggerEnter2D", BindingFlags.NonPublic | BindingFlags.Instance);
    	Assert.NotNull(onTriggerEnter);

    	onTriggerEnter.Invoke(spearHitbox, new object[] { enemyCollider });
    	onTriggerEnter.Invoke(spearHitbox, new object[] { enemyCollider });

    	yield return null;

    	Assert.AreEqual(1, enemy.HitCount);
    }

    [UnityTest]
    public IEnumerator DeadPlayer_CannotAttackWithSpear()
    {
    	EquipSpear();

    	yield return null;

    	playerHealth.TakeDamage(999);

    	yield return null;

    	Assert.IsTrue(playerHealth.IsDead);

    	bool started = playerCombat.TryAttack();

    	Assert.IsFalse(started);
    	Assert.IsFalse(playerCombat.IsAttacking);
    	Assert.IsFalse(spearCollider.enabled);
    }

    [UnityTest]
    public IEnumerator SpearAttackState_ReturnsToValidStateAfterFinishing()
    {
    	EquipSpear();
    	playerCombat.TryAttack();
    	Assert.IsTrue(playerCombat.IsAttacking);

    	yield return WaitForAttackToFinish();

    	Assert.IsFalse(playerCombat.IsAttacking);

    	bool startedAgain = playerCombat.TryAttack();
    	Assert.IsTrue(startedAgain);
    }

    [TestCase(1f, 0f, true, TestName = "SpearHitbox_AttackingRight_IsHorizontal")]
    [TestCase(-1f, 0f, true, TestName = "SpearHitbox_AttackingLeft_IsHorizontal")]
    [TestCase(0f, 1f, false, TestName = "SpearHitbox_AttackingUp_IsVertical")]
    [TestCase(0f, -1f, false, TestName = "SpearHitbox_AttackingDown_IsVertical")]
    public void SpearHitbox_MatchesAttackDirection(float dirX, float dirY, bool expectHorizontal)
    {
    	EquipSpear();
    	Vector2 direction = new Vector2(dirX, dirY);
    	AimAt(direction);

    	playerCombat.TryAttack();

    	Vector2 expectedSize = expectHorizontal ? spearConfig.horizontalHitboxSize : spearConfig.verticalHitboxSize;
    	Assert.AreEqual(expectedSize.x, spearCollider.size.x, 0.001f, "Hitbox width should match the swing direction.");
    	Assert.AreEqual(expectedSize.y, spearCollider.size.y, 0.001f, "Hitbox height should match the swing direction.");

    	Vector2 expectedPosition = direction * spearConfig.attackDistance;
    	Vector2 actualPosition = spearHitbox.transform.localPosition;
    	Assert.AreEqual(expectedPosition.x, actualPosition.x, 0.01f);
    	Assert.AreEqual(expectedPosition.y, actualPosition.y, 0.01f);
    }

    [UnityTest]
    public IEnumerator SpearHitbox_ReshapesBetweenAttacks()
    {
    	EquipSpear();

    	yield return AimAtAndWaitForInput(Vector2.right);
    	Assert.IsTrue(playerCombat.TryAttack());
    	Assert.AreEqual(spearConfig.horizontalHitboxSize.x, spearCollider.size.x, 0.001f);

    	yield return WaitForAttackToFinish();
    	Assert.IsFalse(playerCombat.IsAttacking, "Test setup: expected the first attack to be over.");

    	yield return AimAtAndWaitForInput(Vector2.up);
    	Assert.IsTrue(playerCombat.TryAttack(), "Second attack should start once the first has finished.");
    	Assert.AreEqual(spearConfig.verticalHitboxSize.x, spearCollider.size.x, 0.001f);
    	Assert.AreEqual(spearConfig.verticalHitboxSize.y, spearCollider.size.y, 0.001f);
    }

    [Test]
    public void SwordHitbox_IsNotReshaped_WhenDirectionalIsOff()
    {
    	Vector2 originalSize = swordCollider.size;
    	AimAt(Vector2.up);

    	playerCombat.TryAttack();

    	Assert.AreEqual(originalSize, swordCollider.size);
    }
}
