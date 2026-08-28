# Grid and Tilemapping

The project went through two different tile/ground systems. The **current, actively used one is Unity's built-in Tilemap** (Grid + Tilemap + Tilemap Renderer + Tilemap Collider 2D). The original custom system still exists in the codebase but is no longer wired into the live level — see [[TileGrid (Legacy System)]].

## Current system: Unity Tilemap

### Scene structure
`Grid` GameObject → child `TileMap` GameObject with:
- `Tilemap` + `Tilemap Renderer`
- `Tilemap Collider 2D`
- `Composite Collider 2D` (merges every individual tile's box collider into one continuous seamless shape — critical for preventing the player/enemy from snagging on tiny seams between adjacent tile colliders)
- `Rigidbody2D` set to **Static** (required for `Composite Collider 2D` to function)

### `GrayTileGenerator.cs` (`Assets/Scripts/Tiles/Editor/`)
An editor menu tool: **Tools → Dragon Adventure → Generate Gray Tile Asset**. Since there's no real tile art yet, this procedurally creates:
- A small gray `Texture2D`, saved as `Assets/Tiles/GrayTileTexture.png`, imported as a Sprite.
- A `Tile` asset (`Assets/Tiles/GrayTile.asset`) referencing that sprite.

Drag the generated `GrayTile.asset` into a Tile Palette (Window → 2D → Tile Palette) to paint with it using Unity's normal tile-painting workflow.

### `PhysicsMaterialGenerator.cs` (`Assets/Scripts/Common/Editor/`)
Editor menu tool: **Tools → Dragon Adventure → Generate No-Friction Physics Material 2D**. Creates `Assets/Physics/NoFriction.physicsMaterial2D` (0 friction, 0 bounciness). Assigned to the Tilemap's collider and the Player/Enemy colliders to further reduce "sticking" when sliding across the ground, on top of the Composite Collider fix.

### Physics gotchas solved here
- **Seam-snagging**: adjacent tile colliders can catch a moving `Rigidbody2D` even when perfectly aligned, due to floating-point precision at shared vertices. Fixed via `Composite Collider 2D` (merges shapes) + giving character colliders a small `Edge Radius` (rounds corners so they glide over seams) + a zero-friction Physics Material 2D. A code-side "anti-snag" nudge exists in `PlayerController`/`EnemyPatrol` as a last-resort safety net (see [[Player]] and [[Enemy]]).
- **Tilemap must NOT be Dynamic**: if the Tilemap's `Rigidbody2D` is left as `Dynamic` (Unity's default when adding one), gravity pulls the *entire level* down. It must be `Static`.

## Legacy System

The project's original custom grid/tile system (`TileGrid`, gray-cube painter) has been superseded by the Tilemap described above and is no longer used for the live level. Full details are archived in [[TileGrid (Legacy System)]].
