# Player

The player is a `Rigidbody2D`-based 2D character confined to a fixed XY plane (Z is constant). No 3D `CharacterController` is used — everything goes through Unity's 2D physics so it interacts correctly with the `Tilemap`.

## Scripts (`Assets/Scripts/Player/`)

### `PlayerInputHandler.cs`
Reads input from a `PlayerInput` component (Behavior: **Send Messages**) bound to `InputSystem_Actions`. Exposes:
- `MoveInput` (Vector2, only `.x` is used for movement)
- `SprintHeld` (bool)
- `CrouchHeld` (bool)
- `LastJumpPressedTime` (float, a timestamp rather than a simple flag — enables jump buffering)
- `ConsumeJump()` — resets the jump timestamp once a jump has actually been used, so one press can't double-trigger.

### `PlayerController.cs`
Core movement, on `Rigidbody2D`. Key mechanics:
- **Horizontal movement**: sets `rb.linearVelocity.x` directly every `FixedUpdate` (physics-driven, not `Update`, to avoid stutter).
- **Jumping**: uses `Physics2D.gravity.y * rb.gravityScale` plus a jump-height field to compute the exact launch velocity needed to reach that height (`CalculateJumpVelocity`).
- **Coyote time + jump buffering**: `CanJump()` is a pure static function (unit tested) that allows a jump if you're within `coyoteTime` seconds of last being grounded AND within `jumpBufferTime` seconds of pressing jump — this smooths out the classic "jump didn't register" feeling from `Rigidbody2D`/`CharacterController` grounded-flag flakiness.
- **Ground check**: a small `Physics2D.OverlapCircle` at the feet (optionally at a `groundCheck` child Transform, or auto-computed from the collider bounds if not assigned), explicitly ignoring the player's own collider so it can't falsely detect itself.
- **Anti-snag**: if grounded and trying to move but not actually advancing (a known Unity 2D issue where a `Rigidbody2D` can catch on the seam between adjacent tile colliders), it nudges the character upward slightly after a short delay to pop free. This is a safety net — the real fix is rounding the collider's `Edge Radius` and merging the Tilemap's colliders with a `Composite Collider 2D` (see [[Grid and Tilemapping]]).
- **Dialogue pause**: while `DialogueUI.IsOpen` is true, `FixedUpdate` zeroes horizontal velocity and skips jump handling (gravity still applies, so an airborne player still lands), and `Update` skips sprite-facing updates — see [[Dialogue System]].
- **Sprint**: hold left or right Shift to set horizontal speed to exactly `moveSpeed * 2`. Releasing Shift clears sprint state and restores base speed.
- **Crouch**: hold left or right Ctrl to halve the visual's height while keeping its bottom edge at the player's feet. Releasing Ctrl restores the initial full height. This is visual-only; the physics collider remains unchanged, so crouching does not yet allow travel under low ceilings.
- **One-way platform drop-through**: while standing on a `OneWayPlatform` layer surface, press Ctrl or double-tap `S` to briefly ignore the contacted platform collider and fall to the ground below. See [[Grid and Tilemapping]].
- **Fall respawn**: tracks the player's last grounded physics position. If `useFallRespawn` is enabled and the player's Y position drops below `fallRespawnY`, the player is moved back to the last grounded position, offset upward by `respawnYOffset` and sideways opposite their last movement direction by `respawnXOffset`. Velocity is cleared, and movement resumes from there.
- **Game-over restart**: keeps the existing player object, restores full health, and places it at the nearest `LevelSpawnPoint` in the active scene. Every gameplay scene therefore needs at least one spawn point.
- **Hazard respawn**: `SpikeTrap` uses the same safe grounded position after a surviving hit. See [[Hazards]].

### `PlayerHealth.cs`
- 3 max HP (configurable).
- `TakeDamage(amount)`: reduces HP, logs a hit message, fires `OnPlayerHit` / `OnHealthChanged` events (consumed by the HUD), and starts a 1.5s invulnerability window during which further damage is ignored.
- During invulnerability, the sprite flickers (toggles visibility) via `PlayerSpriteVisual`.
- Fires `OnPlayerDefeated` when HP reaches 0 (consumed by `GameOverUI`).
- Returns whether damage was applied. Hazards use this to avoid moving the player again during the invulnerability window.

### `PlayerSpriteVisual.cs`
- Loads `DragonIdle1.png` and `DragonIdle2.png` from `Assets/Resources/Sprites/` and loops the two idle frames during Play Mode.
- The dragon frames are imported as single sprites at 32 pixels per unit with point filtering; use **Tools -> Dragon Adventure -> Configure Dragon Idle Sprites** if the images are reimported with different settings.
- `[ExecuteAlways]` so the first idle frame is visible in the Scene view even outside Play mode.
- `SetFacing(horizontal)` flips the sprite (`flipX`) based on movement direction.
- `SetVisible(bool)` / `IsVisible` — used by `PlayerHealth` for the invulnerability flicker.
- `SetCrouching(bool)` — applies the held-Ctrl visual height change.

## Scene Setup
`Player` GameObject has: `Rigidbody2D` (Dynamic, gravity scale ~3, rotation Z frozen, Interpolate ON), `BoxCollider2D`, `PlayerInput` (Actions = `InputSystem_Actions`, Behavior = Send Messages), `PlayerInputHandler`, `PlayerController`, `PlayerHealth`, `PlayerInteractor` (see [[Interaction System]]), and a `Visual` child containing the `SpriteRenderer` and `PlayerSpriteVisual`. Tag = `Player`, Layer = `Player` (custom layer, used so enemies don't physically collide with the player — see [[Enemy]]).

The player visual uses the `Characters` sorting layer, which renders above the world's `Default` sorting layer so tiles cannot obscure the player.

## Initial Spawn

`Persistent` owns global systems but does not contain a Player. Each first-entry gameplay scene must contain a scene-owned `Player` prefab instance. The current `PlayersHouse` entry scene places its Player at the `LevelSpawnPoint` location; later scene transitions retain that Player with `DontDestroyOnLoad` and move it to their destination spawn point.

For pits/falling off the map, tune `PlayerController.fallRespawnY` in the Inspector. Set it below the playable level floor; when the player drops past that Y value, they respawn at the last grounded spot, offset away from their last movement direction by `respawnXOffset` and upward by `respawnYOffset`.
