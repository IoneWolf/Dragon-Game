using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Displays the player's two-frame dragon idle animation and flips it to face movement direction.
// ExecuteAlways so the first idle frame is visible in the Scene view even before pressing Play.
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteVisual : MonoBehaviour
{
    private const string CharacterSortingLayerName = "Characters";
    private const string IdleSpriteResourcesPath = "Sprites";
    private const float BaseVisualYOffset = 0.5f;

    [Tooltip("Legacy placeholder color. The dragon idle sprites use their source colors instead.")]
    public Color spriteColor = Color.blue;
    [Tooltip("Standing width and height multiplier for the dragon sprite in world units.")]
    public float size = 1f;
    [Tooltip("Idle animation frames played per second while the player is not moving.")]
    public float idleFramesPerSecond = 3f;

    private SpriteRenderer spriteRenderer;
    private bool isCrouching;
    private Sprite[] idleFrames;
    private float idleAnimationStartTime;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        LoadIdleFrames();
        idleAnimationStartTime = Time.time;
        Refresh();
    }

    private void Update()
    {
        if (!Application.isPlaying || idleFrames == null || idleFrames.Length == 0)
            return;

        int frameIndex = Mathf.FloorToInt((Time.time - idleAnimationStartTime) * idleFramesPerSecond) % idleFrames.Length;
        spriteRenderer.sprite = idleFrames[frameIndex];
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall -= DelayedRefresh;
        EditorApplication.delayCall += DelayedRefresh;
#endif
    }

#if UNITY_EDITOR
    private void DelayedRefresh()
    {
        if (this == null) return;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        Refresh();
    }
#endif

    private void Refresh()
    {
        LoadIdleFrames();
        if (idleFrames != null && idleFrames.Length > 0)
            spriteRenderer.sprite = idleFrames[0];

        spriteRenderer.color = Color.white;
        spriteRenderer.sortingLayerName = CharacterSortingLayerName;
        spriteRenderer.sortingOrder = 0;

        float crouchScale = isCrouching ? 0.5f : 1f;
        transform.localScale = new Vector3(size, isCrouching ? size * 0.5f : size, size);
        transform.localPosition = Vector3.up * BaseVisualYOffset + Vector3.down * (size * (1f - crouchScale) * 0.5f);
    }

    private void LoadIdleFrames()
    {
        if (idleFrames != null && idleFrames.Length > 0)
            return;

        Sprite[] sprites = Resources.LoadAll<Sprite>(IdleSpriteResourcesPath);
        System.Array.Sort(sprites, (left, right) => string.CompareOrdinal(left.name, right.name));

        idleFrames = System.Array.FindAll(sprites, sprite =>
            sprite.name.StartsWith("DragonIdle1") || sprite.name.StartsWith("DragonIdle2"));
    }

    // Called by PlayerController with the horizontal input axis.
    public void SetFacing(float horizontal)
    {
        spriteRenderer.flipX = horizontal < 0f;
    }

    public void SetCrouching(bool crouching)
    {
        if (isCrouching == crouching) return;

        isCrouching = crouching;
        Refresh();
    }

    // Used by PlayerHealth to flicker the sprite while invulnerable.
    public bool IsVisible => spriteRenderer != null && spriteRenderer.enabled;

    public void SetVisible(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
    }
}
