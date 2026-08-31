using UnityEngine;

[AddComponentMenu("Dragon Adventure/Hazards/Spike Trap")]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PolygonCollider2D))]
public class SpikeTrap : MonoBehaviour
{
    [Tooltip("Hit points removed when the player touches this trap.")]
    public int damage = 1;
    [Tooltip("Color of the generated spike sprite.")]
    public Color spikeColor = new Color(0.85f, 0.85f, 0.9f, 1f);

    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null)
            spriteRenderer.sprite = CreateTriangleSprite();

        spriteRenderer.color = spikeColor;
        ConfigureCollider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageAndRespawnPlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        DamageAndRespawnPlayer(other);
    }

    private void DamageAndRespawnPlayer(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (playerHealth == null || player == null || !playerHealth.TakeDamage(damage))
            return;

        if (playerHealth.CurrentHP > 0)
            player.RespawnAtLastGroundedPosition();
    }

    private void ConfigureCollider()
    {
        PolygonCollider2D spikeCollider = GetComponent<PolygonCollider2D>();
        spikeCollider.isTrigger = true;
        spikeCollider.points = new[]
        {
            new Vector2(-0.5f, -0.5f),
            new Vector2(0f, 0.5f),
            new Vector2(0.5f, -0.5f)
        };
    }

    private static Sprite CreateTriangleSprite()
    {
        const int textureSize = 32;
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < textureSize; y++)
        {
            float halfWidth = (textureSize - y) * 0.5f;
            float center = (textureSize - 1f) * 0.5f;
            for (int x = 0; x < textureSize; x++)
            {
                bool insideTriangle = Mathf.Abs(x - center) <= halfWidth;
                texture.SetPixel(x, y, insideTriangle ? Color.white : Color.clear);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, textureSize, textureSize), new Vector2(0.5f, 0f), textureSize);
    }
}
