using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    
    [Header("Hit Effect")]
    [SerializeField] private float hitEffectSize = 0.15f;
    [SerializeField] private float hitEffectLifetime = 0.15f;
    [SerializeField] private string hitEffectSortingLayer = "Player";
    
    public int Damage => damage;
    
    private static Sprite whiteSquareSprite;
    
    private readonly HashSet<IsDamageable> hitThisSwing = new HashSet<IsDamageable>();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
    	if (other.TryGetComponent(out IsDamageable target) && !hitThisSwing.Contains(target))
    	{
    		target.TakeDamage(damage);
    		hitThisSwing.Add(target);
    		
    		SpawnEffect(other.ClosestPoint(transform.position));
    	}
    }
    
    public void ResetSwing()
    {
    	hitThisSwing.Clear();
    }
    
    public void PlaySwingEffect()
    {
    	SpawnEffect(transform.position);
    }
    
    private void SpawnEffect(Vector2 position)
    {
    	if (whiteSquareSprite == null)
    	{
    		Texture2D texture = new Texture2D(1, 1);
    		texture.SetPixel(0, 0, Color.white);
    		texture.Apply();
    		whiteSquareSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2 (0.5f, 0.5f), 1f);
    	}
    	
    	GameObject effect = new GameObject("HitEffect");
    	effect.transform.position = position;
    	effect.transform.localScale = Vector3.one * hitEffectSize;
    	
    	SpriteRenderer sr = effect.AddComponent<SpriteRenderer>();
    	sr.sprite = whiteSquareSprite;
    	sr.sortingLayerName = hitEffectSortingLayer;
    	sr.sortingOrder = 100;
    	
    	Destroy(effect, hitEffectLifetime);
    }
}
