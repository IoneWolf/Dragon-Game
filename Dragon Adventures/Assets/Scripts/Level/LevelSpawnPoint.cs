using UnityEngine;

// Place one in a scene and give it a spawnId that matches a LevelExit targetSpawnId.
[AddComponentMenu("Dragon Adventure/Level/Level Spawn Point")]
[RequireComponent(typeof(SpriteRenderer))]
public class LevelSpawnPoint : MonoBehaviour
{
    public string spawnId = "Default";
    public bool connectMainCameraToPlayer = true;
    public Vector2 cameraOffset = new Vector2(0f, 1f);

    [Header("Marker")]
    public Color markerColor = new Color(0.15f, 0.9f, 0.7f, 1f);
    public int markerSortingOrder = 10;

    private void Awake()
    {
        SpriteRenderer markerRenderer = GetComponent<SpriteRenderer>();
        if (markerRenderer == null)
            markerRenderer = gameObject.AddComponent<SpriteRenderer>();

        if (markerRenderer.sprite == null)
            markerRenderer.sprite = SquareSpriteFactory.CreateSquareSprite(Color.white);

        markerRenderer.color = markerColor;
        markerRenderer.sortingOrder = markerSortingOrder;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.35f);
        Gizmos.DrawLine(transform.position + Vector3.left * 0.5f, transform.position + Vector3.right * 0.5f);
    }
}