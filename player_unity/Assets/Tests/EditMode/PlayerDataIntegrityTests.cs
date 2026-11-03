using NUnit.Framework;
using UnityEngine;

public class PlayerDataIntegrityTests
{
    private GameObject managerObject;
    private PlayerDataManager manager;

    [SetUp]
    public void SetUp()
    {
        managerObject = new GameObject("PlayerDataManager");
        manager = managerObject.AddComponent<PlayerDataManager>();
        manager.InitData();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(managerObject);
    }

    [Test]
    public void ValidLevel_IsAccepted()
    {
        bool result = manager.TrySetPlayerLevel(5);

        Assert.IsTrue(result);
        Assert.AreEqual(5, manager.Data.level);
    }
    [Test]
    public void NegativeLevel_IsRejected()
    {
        manager.TrySetPlayerLevel(5);

        bool result = manager.TrySetPlayerLevel(-10);

        Assert.IsFalse(result);
        Assert.AreEqual(5, manager.Data.level);
    }
    [Test]
    public void ZeroLevel_IsRejected()
    {
        manager.TrySetPlayerLevel(5);

        bool result = manager.TrySetPlayerLevel(0);

        Assert.IsFalse(result);
        Assert.AreEqual(5, manager.Data.level);
    }
    
}
