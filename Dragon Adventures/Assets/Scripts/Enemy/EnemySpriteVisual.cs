using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
}
