# Game Overview

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

## Current State

This is a placeholder-art prototype — the player (blue square) and enemies (red squares) are procedurally generated solid-color sprites rather than real art, so the whole gameplay loop can be tested and iterated on before any art pass. See [[Asset Requirements]] for what's still needed.

For how each system works under the hood, see the notes in `02 - Systems/`, starting with [[Player]], [[Camera]], [[Grid and Tilemapping]], [[Enemy]], and [[HUD]].
