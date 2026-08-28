# TileGrid (Legacy System)

**Status:** Deprecated / not used in the live level. Superseded by Unity's built-in Tilemap — see [[Grid and Tilemapping]].

Before switching to the real Tilemap, the project used a fully custom grid system:

- `TileGrid.cs` (`Assets/Scripts/Tiles/`) — a `MonoBehaviour` holding a flattened `bool[]` grid; `GenerateGrid()`/`RefreshTile()` spawn/despawn actual gray `Cube` primitives per active cell on the object's local XY plane.
- `TileGridEditor.cs` (`Assets/Scripts/Tiles/Editor/`) — a custom Inspector with a clickable button grid to paint/erase cells, plus a **Paint In Scene View** mode (left-click paint, Shift+Left-click erase, right-click/Alt-drag left alone for camera navigation) using a claimed `GUIUtility.hotControl` so drag-painting works. Also draws a yellow wireframe gizmo of the grid's bounds and has a "Frame Grid In Scene View" button, useful for laying out long levels.

This system is not currently used for the live level but is still in the project and functional if you want a fast, code-driven grid without needing tile art. Safe to delete once you're confident the Tilemap workflow covers everything you need — see [[Development Roadmap]].
