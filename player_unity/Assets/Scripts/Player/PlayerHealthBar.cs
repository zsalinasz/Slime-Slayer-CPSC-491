using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerHealthBar : MonoBehaviour
{
    [Header("Sprite")]
    [Tooltip("Your lifebar.png (imported as a Sprite).")]
    [SerializeField] private Sprite barSprite;

    [Header("Layout")]
    [Tooltip("Where the bar sits relative to the player's pivot, in world units.")]
    [SerializeField] private Vector2 offset = new Vector2(0f, 0.8f);
    [Tooltip("Bar width in world units. Height follows the sprite's own aspect ratio so it never looks stretched.")]
    [SerializeField] private float barWidth = 1f;

    [Header("Look")]
    [Tooltip("Tint on the part of the bar that shows remaining health. White = the sprite's own colors.")]
    [SerializeField] private Color fillTint = Color.white;
    [Tooltip("Tint on the dimmed copy that shows through where health has been lost.")]
    [SerializeField] private Color emptyTint = new Color(0.25f, 0.25f, 0.25f, 1f);

    [Header("Sorting")]
    [Tooltip("Drawn this many sorting orders above the player's sprite so the bar isn't hidden behind it.")]
    [SerializeField] private int sortingOrderOffset = 10;

    private PlayerHealth playerHealth;
    private Transform barRoot;
    private SpriteRenderer fillRenderer;

    private int spritePixelWidth;

    private Sprite[] fillSprites;

    private void Awake()
    {
        if (barSprite == null)
        {
            Debug.LogError("PlayerHealthBar: assign your lifebar sprite to 'Bar Sprite' in the Inspector.", this);
            return;
        }

        playerHealth = GetComponent<PlayerHealth>();

        BuildBar();

        if (playerHealth.OnHealthChanged != null)
            playerHealth.OnHealthChanged.AddListener(SetHealth);
    }

    private void OnDestroy()
    {
        if (playerHealth != null && playerHealth.OnHealthChanged != null)
            playerHealth.OnHealthChanged.RemoveListener(SetHealth);

        if (barRoot != null)
            Destroy(barRoot.gameObject);

        if (fillSprites != null)
        {
            foreach (Sprite s in fillSprites)
            {
                if (s != null) Destroy(s);
            }
        }
    }

    private void LateUpdate()
    {
        if (barRoot != null)
            barRoot.position = transform.position + (Vector3)offset;
    }

    private void BuildBar()
    {
        Rect rect = barSprite.rect;
        spritePixelWidth = Mathf.RoundToInt(rect.width);
        fillSprites = new Sprite[spritePixelWidth + 1];

        float scale = barWidth / (rect.width / barSprite.pixelsPerUnit);

        SpriteRenderer playerSprite = GetComponentInChildren<SpriteRenderer>();
        int layerId = playerSprite != null ? playerSprite.sortingLayerID : 0;
        int baseOrder = (playerSprite != null ? playerSprite.sortingOrder : 0) + sortingOrderOffset;

        barRoot = new GameObject("PlayerHealthBar").transform;
        barRoot.position = transform.position + (Vector3)offset;

        GameObject empty = new GameObject("Empty");
        empty.transform.SetParent(barRoot, false);
        empty.transform.localPosition = -barSprite.bounds.center * scale;
        empty.transform.localScale = new Vector3(scale, scale, 1f);

        SpriteRenderer emptyRenderer = empty.AddComponent<SpriteRenderer>();
        emptyRenderer.sprite = barSprite;
        emptyRenderer.color = emptyTint;
        emptyRenderer.sortingLayerID = layerId;
        emptyRenderer.sortingOrder = baseOrder;

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(barRoot, false);
        fill.transform.localPosition = new Vector3(-barWidth * 0.5f, 0f, 0f);
        fill.transform.localScale = new Vector3(scale, scale, 1f);

        fillRenderer = fill.AddComponent<SpriteRenderer>();
        fillRenderer.color = fillTint;
        fillRenderer.sortingLayerID = layerId;
        fillRenderer.sortingOrder = baseOrder + 1;

        SetHealth(1, 1);
    }

    private Sprite GetFillSprite(int pixelWidth)
    {
        if (pixelWidth <= 0) return null;

        if (fillSprites[pixelWidth] == null)
        {
            Rect rect = barSprite.rect;
            fillSprites[pixelWidth] = Sprite.Create(
                barSprite.texture,
                new Rect(rect.x, rect.y, pixelWidth, rect.height),
                new Vector2(0f, 0.5f),
                barSprite.pixelsPerUnit);
        }

        return fillSprites[pixelWidth];
    }

    private void SetHealth(int current, int max)
    {
        if (barRoot == null || fillRenderer == null) return;

        float percent = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;

        int pixels = current >= max ? spritePixelWidth : Mathf.FloorToInt(percent * spritePixelWidth);
        if (current > 0) pixels = Mathf.Max(pixels, 1);

        fillRenderer.sprite = GetFillSprite(pixels);

        barRoot.gameObject.SetActive(current > 0);
    }
}
