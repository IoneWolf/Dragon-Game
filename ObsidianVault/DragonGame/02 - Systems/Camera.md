# Camera

## `CameraFollow.cs` (`Assets/Scripts/Camera/`)

A simple fixed-plane 2D follow camera:
- Tracks the target's **X and Y** position only.
- Keeps its **own Z depth** constant (captured once in `Awake()`), so it never drifts closer/further from the play plane.
- **Never rotates** — this is a deliberate "fixed 2D plane" design (the project originally experimented with a rotating/angled 3D-style follow camera, but was changed to a straightforward flat 2D camera to match the platformer's movement plane).
- Uses `Vector3.SmoothDamp` for gentle lag/smoothing behind the target rather than snapping instantly.

## Fields
- `target` — the Transform to follow (set to `Player`).
- `offset` — a `Vector2` offset from the target (e.g. to frame slightly ahead/above).
- `smoothTime` — smoothing responsiveness (lower = snappier, higher = laggier/smoother).

## Scene Setup
Attached to `Main Camera`. Camera itself is typically Orthographic for a clean 2D look, positioned at some negative Z (e.g. `-10`) so it looks toward the play plane at `Z = 0`.

## Known Issue (resolved)
Early on, the scene accidentally ended up with **two** `Main Camera` GameObjects (one properly tagged `MainCamera`, one untagged with `CameraFollow` attached and stale serialized field values from an earlier version of the script). This caused the camera to appear off-center/not tracking correctly. If camera behavior ever seems to desync from what the script logic implies, check the Hierarchy for duplicate `Main Camera` objects first.
