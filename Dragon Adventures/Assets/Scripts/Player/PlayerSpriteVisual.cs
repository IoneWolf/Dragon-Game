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
    public Color spriteColor = Color.blue;
    public float size = 1f;

    private SpriteRenderer spriteRenderer;

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
        transform.localScale = Vector3.one * size;
    }

    // Called by PlayerController with the horizontal input axis.
    public void SetFacing(float horizontal)
    {
        spriteRenderer.flipX = horizontal < 0f;
    }

    // Used by PlayerHealth to flicker the sprite while invulnerable.
    public bool IsVisible => spriteRenderer != null && spriteRenderer.enabled;

    public void SetVisible(bool visible)
    {
        if (spriteRenderer != null) spriteRenderer.enabled = visible;
    }
}
