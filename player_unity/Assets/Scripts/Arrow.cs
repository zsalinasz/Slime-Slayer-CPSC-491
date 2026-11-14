using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Arrow : MonoBehaviour
{
    private static readonly Vector2 PlaceholderSize = new Vector2(0.5f, 0.12f);
    private static Sprite whiteSquareSprite;

    private Rigidbody2D rb;
    private Transform owner;
    private int damage;
    private bool hasHit;

    public int Damage => damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, int damage, float speed, float lifetime, Transform owner)
    {
        this.damage = damage;
        this.owner = owner;

        direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;

        transform.right = direction;
        rb.linearVelocity = direction * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) 
        	return;

        if (owner != null && other.transform.IsChildOf(owner)) 
        	return;

        IsDamageable target = other.GetComponentInParent<IsDamageable>();
        if (target != null)
        {
            hasHit = true;
            target.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (!other.isTrigger)
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }

    public static Arrow CreatePlaceholder(Vector2 position, int sortingLayerId, int sortingOrder)
    {
        if (whiteSquareSprite == null)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            whiteSquareSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        }

        GameObject arrowObject = new GameObject("Arrow");
        arrowObject.transform.position = position;

        Rigidbody2D body = arrowObject.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;
        body.gravityScale = 0f;

        BoxCollider2D collider = arrowObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = PlaceholderSize;

        GameObject visual = new GameObject("Visual");
        visual.transform.SetParent(arrowObject.transform, false);
        visual.transform.localScale = new Vector3(PlaceholderSize.x, PlaceholderSize.y, 1f);

        SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
        sr.sprite = whiteSquareSprite;
        sr.color = new Color(0.6f, 0.4f, 0.2f, 1f);
        sr.sortingLayerID = sortingLayerId;
        sr.sortingOrder = sortingOrder;

        return arrowObject.AddComponent<Arrow>();
    }
}
