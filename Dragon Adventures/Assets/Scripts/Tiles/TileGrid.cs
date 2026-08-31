using UnityEngine;

// A simple grid of toggleable gray cube "ground" tiles, editable in the Inspector.
[ExecuteAlways]
public class TileGrid : MonoBehaviour
{
    [Header("Grid Size")]
    [Tooltip("Number of tile cells along the local X axis.")]
    public int width = 8;
    [Tooltip("Number of tile cells along the local Y axis.")]
    public int height = 8;
    [Tooltip("Width and height of each tile cell in world units.")]
    public float cellSize = 1f;

    [Header("Tile Look")]
    [Tooltip("Color used for generated legacy grid tiles.")]
    public Color tileColor = Color.gray;

    // Flattened width*height map; true = a ground tile exists at that cell.
    [HideInInspector] public bool[] tiles;

    private Transform tilesParent;

    private void OnEnable() => EnsureMapSize();

    private void OnDrawGizmosSelected()
    {
        // Outline of the full grid area, handy for seeing the extents of a long map while painting.
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Vector3 size = new Vector3(width * cellSize, height * cellSize, 0.1f);
        Vector3 center = transform.TransformPoint(new Vector3(width * cellSize / 2f, height * cellSize / 2f, 0f));
        Gizmos.matrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
        Gizmos.matrix = Matrix4x4.identity;
    }

    public void EnsureMapSize()
    {
        int required = Mathf.Max(0, width * height);
        if (tiles != null && tiles.Length == required) return;

        bool[] resized = new bool[required];
        if (tiles != null)
        {
            for (int i = 0; i < Mathf.Min(tiles.Length, required); i++)
                resized[i] = tiles[i];
        }
        tiles = resized;
    }

    public int IndexOf(int x, int y) => y * width + x;

    public bool GetTile(int x, int y)
    {
        EnsureMapSize();
        if (x < 0 || y < 0 || x >= width || y >= height) return false;
        return tiles[IndexOf(x, y)];
    }

    public void SetTile(int x, int y, bool value)
    {
        EnsureMapSize();
        if (x < 0 || y < 0 || x >= width || y >= height) return;
        tiles[IndexOf(x, y)] = value;
        RefreshTile(x, y);
    }

    public void ToggleTile(int x, int y) => SetTile(x, y, !GetTile(x, y));

    private Transform GetOrCreateParent()
    {
        if (tilesParent != null) return tilesParent;

        Transform existing = transform.Find("Tiles");
        tilesParent = existing != null ? existing : new GameObject("Tiles").transform;
        tilesParent.SetParent(transform, false);
        return tilesParent;
    }

    private Material CreateTileMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        Material mat = new Material(shader) { color = tileColor };
        return mat;
    }

    public void RefreshTile(int x, int y)
    {
        Transform parent = GetOrCreateParent();
        string tileName = $"Tile_{x}_{y}";
        Transform existing = parent.Find(tileName);
        bool shouldExist = GetTile(x, y);

        if (shouldExist && existing == null)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = tileName;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = new Vector3(x * cellSize, y * cellSize, 0f);
            cube.transform.localScale = Vector3.one * cellSize;
            cube.GetComponent<Renderer>().sharedMaterial = CreateTileMaterial();
        }
        else if (!shouldExist && existing != null)
        {
            DestroyTile(existing.gameObject);
        }
    }

    public void GenerateGrid()
    {
        EnsureMapSize();
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (GetTile(x, y)) RefreshTile(x, y);
    }

    public void FillAll(bool value)
    {
        EnsureMapSize();
        for (int i = 0; i < tiles.Length; i++) tiles[i] = value;
        RebuildAll();
    }

    public void RebuildAll()
    {
        ClearGrid();
        GenerateGrid();
    }

    public void ClearGrid()
    {
        Transform parent = GetOrCreateParent();
        for (int i = parent.childCount - 1; i >= 0; i--)
            DestroyTile(parent.GetChild(i).gameObject);
    }

    // Converts a point on this grid's local XY plane (Z ignored) into cell coordinates, for scene-view painting.
    public Vector2Int WorldToCell(Vector3 worldPoint)
    {
        Vector3 local = transform.InverseTransformPoint(worldPoint);
        int x = Mathf.FloorToInt(local.x / cellSize + 0.5f);
        int y = Mathf.FloorToInt(local.y / cellSize + 0.5f);
        return new Vector2Int(x, y);
    }

    private void DestroyTile(GameObject go)
    {
        if (Application.isPlaying) Destroy(go);
        else DestroyImmediate(go);
    }
}
