# Hazards

## Spike Trap

`SpikeTrap.prefab` is a reusable upward-pointing triangle hazard.

### Setup

1. Drag `SpikeTrap.prefab` from `Assets/Prefabs` into a level scene.
2. Position the spike on the floor, with its point facing upward.
3. Duplicate or scale placed instances to create longer spike sections.

The prefab supplies a `SpriteRenderer` and a matching triangle-shaped trigger collider. `SpikeTrap` generates the basic triangle sprite at runtime, so no separate art asset is required yet.

### Player Behavior

When a player touches a spike:

- The player takes one point of damage through `PlayerHealth`.
- If the hit does not defeat the player, `PlayerController` immediately returns them to their most recently recorded grounded position, using its existing sideways/upward safety offset.
- The player becomes temporarily invulnerable through the existing health system, preventing repeat damage while being moved away.
- A defeated player uses the existing game-over flow instead of respawning from the spike.

This keeps the safe respawn behavior shared between falling and spike hazards.
