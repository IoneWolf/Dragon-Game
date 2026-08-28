using UnityEditor;
using UnityEngine;

// Inspector tool: click cells to paint/erase gray ground cubes on the TileGrid.
// Also supports painting directly in the Scene view (toggle "Paint In Scene View" below).
[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor
{
    // Above this many cells, the button grid gets slow/unusable - use Scene view painting instead.
    private const int MaxButtonGridCells = 400;

    private bool paintInSceneView;

    public override void OnInspectorGUI()
    {
        TileGrid grid = (TileGrid)target;
        DrawDefaultInspector();
        grid.EnsureMapSize();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Ground Painter", EditorStyles.boldLabel);

        paintInSceneView = GUILayout.Toggle(paintInSceneView, "Paint In Scene View", "Button", GUILayout.Height(24));
        EditorGUILayout.HelpBox(paintInSceneView
            ? "Left-click (or click+drag) in the Scene view to paint tiles. Hold Shift+Left-click to erase. Alt+drag/right-click still orbit the camera normally."
            : "Click a cell below to toggle a gray ground cube on/off, or enable Scene view painting.",
            MessageType.Info);

        if (GUILayout.Button("Frame Grid In Scene View"))
            FrameGridInSceneView(grid);

        bool gridTooBigForButtons = grid.width * grid.height > MaxButtonGridCells;
        if (gridTooBigForButtons)
        {
            EditorGUILayout.HelpBox(
                $"Grid is {grid.width}x{grid.height} ({grid.width * grid.height} cells) - too large for the button grid. " +
                "Use 'Paint In Scene View' above to draw long maps instead.", MessageType.Warning);
        }
        else
        {
            for (int y = grid.height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < grid.width; x++)
                {
                    bool on = grid.GetTile(x, y);
                    GUI.backgroundColor = on ? Color.gray : Color.white;
                    if (GUILayout.Button(GUIContent.none, GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        Undo.RegisterCompleteObjectUndo(grid, "Toggle Tile");
                        grid.ToggleTile(x, y);
                        EditorUtility.SetDirty(grid);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Fill All"))
        {
            Undo.RegisterCompleteObjectUndo(grid, "Fill Grid");
            grid.FillAll(true);
            EditorUtility.SetDirty(grid);
        }
        if (GUILayout.Button("Clear All"))
        {
            Undo.RegisterCompleteObjectUndo(grid, "Clear Grid");
            grid.FillAll(false);
            EditorUtility.SetDirty(grid);
        }
        if (GUILayout.Button("Rebuild"))
        {
            grid.RebuildAll();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnSceneGUI()
    {
        if (!paintInSceneView) return;

        TileGrid grid = (TileGrid)target;
        Event e = Event.current;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        // Right-click and Alt+drag are left alone so Scene view camera look/orbit still works.
        if (e.alt || e.button != 0) return;

        EventType typeForControl = e.GetTypeForControl(controlId);

        if (typeForControl == EventType.MouseDown)
        {
            GUIUtility.hotControl = controlId;
            PaintCell(grid, e);
            e.Use();
        }
        else if (typeForControl == EventType.MouseDrag && GUIUtility.hotControl == controlId)
        {
            PaintCell(grid, e);
            e.Use();
        }
        else if (typeForControl == EventType.MouseUp && GUIUtility.hotControl == controlId)
        {
            GUIUtility.hotControl = 0;
            e.Use();
        }
        else if (typeForControl == EventType.Layout)
        {
            // Claim this control at default priority so clicks reach us instead of picking other objects.
            HandleUtility.AddDefaultControl(controlId);
        }
    }

    private static void PaintCell(TileGrid grid, Event e)
    {
        Plane plane = new Plane(grid.transform.forward, grid.transform.position);
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (!plane.Raycast(ray, out float distance)) return;

        Vector3 worldPoint = ray.GetPoint(distance);
        Vector2Int cell = grid.WorldToCell(worldPoint);
        bool paint = !e.shift; // Left-click paints, Shift+Left-click erases.

        Undo.RegisterCompleteObjectUndo(grid, paint ? "Paint Tile" : "Erase Tile");
        grid.SetTile(cell.x, cell.y, paint);
        EditorUtility.SetDirty(grid);
        SceneView.RepaintAll();
    }

    private static void FrameGridInSceneView(TileGrid grid)
    {
        Vector3 center = grid.transform.TransformPoint(new Vector3(grid.width * grid.cellSize / 2f, grid.height * grid.cellSize / 2f, 0f));
        float largestSide = Mathf.Max(grid.width, grid.height) * grid.cellSize;
        Bounds bounds = new Bounds(center, Vector3.one * Mathf.Max(largestSide, grid.cellSize));

        SceneView view = SceneView.lastActiveSceneView;
        if (view != null) view.Frame(bounds, false);
    }
}
