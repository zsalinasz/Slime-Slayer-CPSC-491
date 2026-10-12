using UnityEngine;

// Test file that serves as a stand in for a real enemies health system
// for combat tests specifically

public class TestDamageable : MonoBehaviour, IsDamageable
{
    public int HitCount { get; private set; }
    public int LastDamageReceived { get; private set; }
    
    public void TakeDamage(int amount)
    {
    	HitCount++;
    	LastDamageReceived = amount;
    }
}
