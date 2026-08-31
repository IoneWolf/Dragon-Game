using UnityEngine;

public enum CameraBoundsCornerType
{
    BottomLeft,
    TopRight
}

// Scene marker used by CameraFollow to define the visible camera bounds rectangle.
[ExecuteAlways]
[AddComponentMenu("Dragon Adventure/Camera/Camera Bounds Corner")]
[RequireComponent(typeof(SpriteRenderer))]
public class CameraBoundsCorner : MonoBehaviour
{
    [Tooltip("Which rectangle corner this marker represents. Place one Bottom Left and one Top Right marker in each gameplay scene.")]
    public CameraBoundsCornerType cornerType = CameraBoundsCornerType.BottomLeft;
    [Tooltip("Editor-only marker color. The marker is hidden during gameplay.")]
    public Color markerColor = new Color(1f, 0.6f, 0.1f, 1f);
    [Tooltip("Marker size in world units shown in the Scene view.")]
    public float markerSize = 0.25f;

    private SpriteRenderer spriteRenderer;

    private void OnEnable()
    {
        Refresh();
    }

    private void OnValidate()
    {
        Refresh();
    }

    private void Refresh()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer.sprite == null)
            spriteRenderer.sprite = SquareSpriteFactory.CreateSquareSprite(Color.white);

        spriteRenderer.color = markerColor;
        spriteRenderer.sortingOrder = 100;
        spriteRenderer.enabled = !Application.isPlaying;
        transform.localScale = Vector3.one * markerSize;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = markerColor;
        Gizmos.DrawWireCube(transform.position, Vector3.one * markerSize);
    }
}
