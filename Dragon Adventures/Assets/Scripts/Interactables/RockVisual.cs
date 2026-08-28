using UnityEngine;

// Generates a solid-color half-circle placeholder sprite (a "rock") for the interactable.
// ExecuteAlways so it's visible in the Scene view without pressing Play, like PlayerSpriteVisual/EnemySpriteVisual.
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class RockVisual : MonoBehaviour
{
    public Color rockColor = Color.black;
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
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        Refresh();
    }

    private void Refresh()
    {
        if (spriteRenderer.sprite == null)
            spriteRenderer.sprite = HalfCircleSpriteFactory.CreateHalfCircleSprite(rockColor);
        transform.localScale = Vector3.one * size;
    }
}
