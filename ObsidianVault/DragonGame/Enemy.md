# Enemy

Enemies are simple patrolling hazards that can either damage the player (side contact) or be defeated by the player (stomped from above), Mario-style.

## Scripts (`Assets/Scripts/Enemy/`)

### `EnemySpriteVisual.cs`
Same idea as `PlayerSpriteVisual` — procedurally generates a solid red square sprite via `SquareSpriteFactory`, `[ExecuteAlways]` so it's visible in the Editor without pressing Play.

### `EnemyPatrol.cs`
Walks back and forth on a `Rigidbody2D` (Dynamic, so gravity applies like the player).
- Every `FixedUpdate`, checks for a wall ahead using a **static `Physics2D.OverlapBox`** just past its leading edge (not a sweeping `BoxCast` — a `BoxCast` can fail to detect a wall it's already touching, which caused the enemy to "cling" to walls instead of turning around).
- On hitting a wall: flips `direction` and rotates 180° on Y so it visually faces the other way.
- Automatically excludes the `Player` layer from its obstacle detection (`obstacleLayerMask`) — otherwise a stationary/approaching player would be treated as a "wall" and cause an unwanted flip that looked like bouncing off them.
- Has the same anti-snag nudge safety net as `PlayerController` (see [[Player]] and [[Grid and Tilemapping]]).

### `EnemyHealth.cs`
Minimal HP tracker (`maxHP`, default 1). `TakeDamage()` reduces HP and destroys the enemy GameObject once it hits 0. Fires `OnHealthChanged` / `OnDefeated` events if other systems want to react (e.g. score, VFX later).

### `Enemy.cs`
The contact/damage logic. Key design point: **the enemy needs to stand solidly on the ground/walls, but must NOT physically collide with the Player** (they should phase through each other), while still reliably detecting contact for damage. This required a few layered fixes:

1. **Two colliders, one GameObject.** The enemy's original `BoxCollider2D` stays solid (`isTrigger = false`) so `EnemyPatrol` and gravity/ground collision work normally. `Enemy.Awake()` automatically adds a **second** `BoxCollider2D` (same size/offset) and marks *that one* as a trigger — used purely for damage/stomp detection. A single collider can't be both solid and a trigger at once, so this split was necessary.
2. **Excluding the Player from solid collision.** `Enemy.Awake()` calls `solidCollider.excludeLayers |= (1 << playerLayer)` at runtime (using `LayerMask.NameToLayer("Player")`) so the solid collider ignores the Player entirely — no manual "Layer Overrides" Inspector fiddling required, just create a `Player` Layer in Project Settings and assign it to the Player GameObject.
3. **Stomp vs. side-touch detection** (`HandleContact` / `IsStomp`): on trigger contact with something tagged `Player`, it checks whether the player's feet are in the *upper portion* of the enemy's bounds (`stompHeightRatio`, default 0.5 = top half) AND the player is falling (`velocity.y <= ~0`). If both are true, it's a stomp: the enemy takes 1 damage and the player gets a small upward bounce (`stompBounceForce`). Otherwise, it's a side touch: the player takes `damage` (default 1) via `PlayerHealth.TakeDamage()`.

## Scene Setup
`Enemy` GameObject: `SpriteRenderer`, `EnemySpriteVisual`, `BoxCollider2D` (the original — ends up solid), `Rigidbody2D` (Dynamic, gravity scale ~3, rotation Z frozen, Continuous collision detection to avoid falling-through-floor tunneling), `EnemyPatrol`, `Enemy` (auto-adds the second trigger collider + `EnemyHealth` via `[RequireComponent]`). Layer = `Enemy` (or any layer other than `Player`) — the important part is the *Player* GameObject/Layer setup described above.

## Debugging Aid
`EnemyPatrol` draws a cyan gizmo (line + wire box) showing exactly where its wall-detection check happens — select the `Enemy` GameObject in the Editor (even during Play mode) to visualize it if patrol behavior looks wrong.
