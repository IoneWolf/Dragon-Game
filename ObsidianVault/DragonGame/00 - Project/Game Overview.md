# Game Overview

A 2D platformer built in Unity 6 (`6000.0.46f1`), using the new Input System and Unity 2D physics (`Rigidbody2D` + `Tilemap`).

## How to Play

| Action | Key |
|---|---|
| Move left/right | `A` / `D` or Arrow Keys |
| Jump | `Space` |
| Sprint | `Left Shift` (held) |
| Crouch / drop through one-way platform | `Left Ctrl` or `Right Ctrl` (held) |
| Drop through one-way platform | Double-tap `S` |

**Goal:** Move through the level, avoid or defeat the red enemy squares, and don't let your HP hit 0.

**Combat — Mario-style stomping:**
- Touching an enemy from the **side** damages you (1 HP), then gives you 1.5 seconds of invulnerability (the player sprite flickers during this).
- Jumping **on top of** an enemy (landing on its upper half while falling) damages/kills the enemy instead, and bounces you upward slightly.
- Player and enemies physically **phase through each other** — they never push/collide like walls do. Only the trigger-based hit detection matters.

**Losing:** If your HP reaches 0, a "GAME OVER" screen appears and the game pauses. Click **Restart** to restore the same player at the nearest spawn point in the active scene.

The main menu's **Play** route starts Chapter 1 in `PlayersHouse`; **Playtest** starts the feature-test Level 1 scene.

## Current State

This is an early prototype: the player uses a two-frame dragon idle sprite, while enemies, NPCs, tiles, and most other visuals remain placeholder art. See [[Asset Requirements]] for what's still needed.

For how each system works under the hood, see the notes in `02 - Systems/`, starting with [[Player]], [[Camera]], [[Grid and Tilemapping]], [[Enemy]], and [[HUD]].
