using UnityEngine;

// Generates a solid red square sprite (placeholder art) for the enemy.
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemySpriteVisual : MonoBehaviour
{
    public Color spriteColor = Color.red;
    public float size = 1f;

    private SpriteRenderer spriteRenderer;

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
            spriteRenderer.sprite = SquareSpriteFactory.CreateSquareSprite(spriteColor);
        transform.localScale = Vector3.one * size;
    }
}
