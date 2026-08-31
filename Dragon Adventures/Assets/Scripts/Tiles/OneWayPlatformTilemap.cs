using UnityEngine;
using UnityEngine.Tilemaps;

// Configures a Tilemap Collider as a one-way platform for tiles painted on this TileMap.
[AddComponentMenu("Dragon Adventure/Tiles/One-Way Platform Tilemap")]
[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlatformEffector2D))]
public class OneWayPlatformTilemap : MonoBehaviour
{
    private void Reset()
    {
        ConfigureComponents();
    }

    private void Awake()
    {
        ConfigureComponents();
    }

    private void OnValidate()
    {
        ConfigureComponents();
    }

    private void ConfigureComponents()
    {
        int platformLayer = LayerMask.NameToLayer("OneWayPlatform");
        if (platformLayer >= 0)
            gameObject.layer = platformLayer;

        TilemapCollider2D tilemapCollider = GetComponent<TilemapCollider2D>();
        tilemapCollider.usedByEffector = true;

        Rigidbody2D platformBody = GetComponent<Rigidbody2D>();
        platformBody.bodyType = RigidbodyType2D.Static;

        PlatformEffector2D effector = GetComponent<PlatformEffector2D>();
        if (effector == null)
            effector = gameObject.AddComponent<PlatformEffector2D>();

        effector.useOneWay = true;
        effector.useOneWayGrouping = true;
        effector.surfaceArc = 160f;
    }
}
