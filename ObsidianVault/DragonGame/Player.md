# Player

The player is a `Rigidbody2D`-based 2D character confined to a fixed XY plane (Z is constant). No 3D `CharacterController` is used — everything goes through Unity's 2D physics so it interacts correctly with the `Tilemap`.

## Scripts (`Assets/Scripts/Player/`)

### `PlayerInputHandler.cs`
Reads input from a `PlayerInput` component (Behavior: **Send Messages**) bound to `InputSystem_Actions`. Exposes:
- `MoveInput` (Vector2, only `.x` is used for movement)
- `SprintHeld` (bool)
- `LastJumpPressedTime` (float, a timestamp rather than a simple flag — enables jump buffering)
- `ConsumeJump()` — resets the jump timestamp once a jump has actually been used, so one press can't double-trigger.

### `PlayerController.cs`
Core movement, on `Rigidbody2D`. Key mechanics:
- **Horizontal movement**: sets `rb.linearVelocity.x` directly every `FixedUpdate` (physics-driven, not `Update`, to avoid stutter).
- **Jumping**: uses `Physics2D.gravity.y * rb.gravityScale` plus a jump-height field to compute the exact launch velocity needed to reach that height (`CalculateJumpVelocity`).
- **Coyote time + jump buffering**: `CanJump()` is a pure static function (unit tested) that allows a jump if you're within `coyoteTime` seconds of last being grounded AND within `jumpBufferTime` seconds of pressing jump — this smooths out the classic "jump didn't register" feeling from `Rigidbody2D`/`CharacterController` grounded-flag flakiness.
- **Ground check**: a small `Physics2D.OverlapCircle` at the feet (optionally at a `groundCheck` child Transform, or auto-computed from the collider bounds if not assigned), explicitly ignoring the player's own collider so it can't falsely detect itself.
- **Anti-snag**: if grounded and trying to move but not actually advancing (a known Unity 2D issue where a `Rigidbody2D` can catch on the seam between adjacent tile colliders), it nudges the character upward slightly after a short delay to pop free. This is a safety net — the real fix is rounding the collider's `Edge Radius` and merging the Tilemap's colliders with a `Composite Collider 2D` (see [[Grid and Tilemapping]]).

### `PlayerHealth.cs`
- 3 max HP (configurable).
- `TakeDamage(amount)`: reduces HP, logs a hit message, fires `OnPlayerHit` / `OnHealthChanged` events (consumed by the HUD), and starts a 1.5s invulnerability window during which further damage is ignored.
- During invulnerability, the sprite flickers (toggles visibility) via `PlayerSpriteVisual`.
- Fires `OnPlayerDefeated` when HP reaches 0 (consumed by `GameOverUI`).

### `PlayerSpriteVisual.cs`
- Procedurally generates a solid-color square sprite (default blue) using the shared `SquareSpriteFactory`, so no actual art asset is required yet.
- `[ExecuteAlways]` so the sprite is visible in the Scene view even outside Play mode.
- `SetFacing(horizontal)` flips the sprite (`flipX`) based on movement direction.
- `SetVisible(bool)` / `IsVisible` — used by `PlayerHealth` for the invulnerability flicker.

## Scene Setup
`Player` GameObject has: `Rigidbody2D` (Dynamic, gravity scale ~3, rotation Z frozen, Interpolate ON), `BoxCollider2D`, `PlayerInput` (Actions = `InputSystem_Actions`, Behavior = Send Messages), `PlayerInputHandler`, `PlayerController`, `PlayerSpriteVisual`, `PlayerHealth`. Tag = `Player`, Layer = `Player` (custom layer, used so enemies don't physically collide with the player — see [[Enemy]]).
