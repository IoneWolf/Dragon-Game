# Dragon Adventure — README

A 2D platformer built in Unity 6 (`6000.0.46f1`), using the new Input System and Unity 2D physics (`Rigidbody2D` + `Tilemap`).

## How to Play

| Action | Key |
|---|---|
| Move left/right | `A` / `D` or Arrow Keys |
| Jump | `Space` |
| Sprint | `Left Shift` (held) |

**Goal:** Move through the level, avoid or defeat the red enemy squares, and don't let your HP hit 0.

**Combat — Mario-style stomping:**
- Touching an enemy from the **side** damages you (1 HP), then gives you 1.5 seconds of invulnerability (the player sprite flickers during this).
- Jumping **on top of** an enemy (landing on its upper half while falling) damages/kills the enemy instead, and bounces you upward slightly.
- Player and enemies physically **phase through each other** — they never push/collide like walls do. Only the trigger-based hit detection matters.

**Losing:** If your HP reaches 0, a "GAME OVER" screen appears and the game pauses. Click **Restart** to reload the level and try again.

## Project Notes

This is a placeholder-art prototype — the player (blue square) and enemies (red squares) are procedurally generated solid-color sprites rather than real art, so the whole gameplay loop can be tested and iterated on before any art pass.

See the other notes in this vault for how each system works:
- [[Player]] — movement, jumping, health, sprite
- [[Camera]] — how the camera follows the player
- [[Grid and Tilemapping]] — how the level/ground is built
- [[Enemy]] — patrol AI, stomp/damage logic
- [[HUD]] — health bar and game-over screen

## Known Rough Edges / Things to Revisit
- Player and Enemy visuals are procedurally generated solid-color squares — swap in real sprites when art is ready (just assign a `Sprite` to the `Sprite Renderer` instead of relying on `PlayerSpriteVisual`/`EnemySpriteVisual`).
- There are two ground/tile systems in the project: an old custom `TileGrid` (gray-cube painter, no longer used for the actual level) and the current real Unity `Tilemap` system. The old one can be deleted once you're confident you don't need it.
- Enemy patrol only reverses on hitting a wall — it doesn't detect ledges/gaps, so it can walk off platform edges.
