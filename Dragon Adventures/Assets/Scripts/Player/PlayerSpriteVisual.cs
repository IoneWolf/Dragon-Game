using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Generates a solid-color square sprite (placeholder art) and flips to face movement direction.
// ExecuteAlways so the sprite is visible in the Scene view even before pressing Play.
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteVisual : MonoBehaviour
{
    [Tooltip("Color used when generating the placeholder player sprite.")]
    public Color spriteColor = Color.blue;
    [Tooltip("Full standing width and height of the placeholder sprite in world units.")]
    public float size = 1f;

    private SpriteRenderer spriteRenderer;
    private bool isCrouching;

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Refresh();
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
        if (spriteRenderer.sprite == null)
            spriteRenderer.sprite = SquareSpriteFactory.CreateSquareSprite(spriteColor);

        float crouchScale = isCrouching ? 0.5f : 1f;
        transform.localScale = new Vector3(size, isCrouching ? size * 0.5f : size, size);
        transform.localPosition = Vector3.down * (size * (1f - crouchScale) * 0.5f);
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
