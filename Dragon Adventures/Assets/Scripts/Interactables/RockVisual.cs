using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Generates a solid-color half-circle placeholder sprite (a "rock") for the interactable.
// ExecuteAlways so it's visible in the Scene view without pressing Play, like PlayerSpriteVisual/EnemySpriteVisual.
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class RockVisual : MonoBehaviour
{
    [Tooltip("Color used when generating the placeholder rock sprite.")]
    public Color rockColor = Color.black;
    [Tooltip("Width and height of the placeholder rock sprite in world units.")]
    public float size = 1f;

    private SpriteRenderer spriteRenderer;

    public Sprite Sprite => spriteRenderer != null ? spriteRenderer.sprite : null;

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
            spriteRenderer.sprite = HalfCircleSpriteFactory.CreateHalfCircleSprite(rockColor);
        transform.localScale = Vector3.one * size;
    }
}
