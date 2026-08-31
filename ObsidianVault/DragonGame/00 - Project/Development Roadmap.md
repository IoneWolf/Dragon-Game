# Development Roadmap

Focused on development order, not a full feature wishlist. See [[NEXT]] for the single active task.

## Working
- Persistent `GameController` for scene transitions and simple run state — see [[Game Controller]]
- Player movement, jumping (coyote time + jump buffering), health/damage, invulnerability flicker — see [[Player]]
- Enemy patrol AI, stomp/damage detection — see [[Enemy]]
- 2D camera follow — see [[Camera]]
- Startup/background music playback — see [[Music]]
- HUD: health bar + game-over screen — see [[HUD]]
- Level ground via Unity Tilemap (Composite Collider, zero-friction physics material) — see [[Grid and Tilemapping]]
- Interact-to-talk: nearest-interactable detection + a reusable dialogue box, with a talking rock as the first example — see [[Interaction System]] and [[Dialogue System]]
- Placeholder NPC with editable dialogue data — see [[NPC]]
- Proximity-triggered level exits with directional scripted walks and named spawn points — see [[Level Transitions]]
- One-way platform tiles with crouch/double-`S` drop-through — see [[Grid and Tilemapping]]
- Camera bounds defined by drag-and-drop corner markers — see [[Camera]]

## In Progress
- Nothing currently marked in progress — see [[NEXT]] once a task is set.

## Planned Later
- Expand the player animation set beyond the current two-frame idle loop — see [[Player]]
- Replace placeholder enemy and tile art — see [[Asset Requirements]]
- Replace placeholder NPC sprite/dialogue with real content — see [[Asset Requirements]]
- Add final background music tracks — see [[Asset Requirements]]
- Add transition polish such as fades/loading screens — see [[Level Transitions]]
- Add ledge/gap detection to enemy patrol so enemies stop walking off platform edges — see [[Enemy]]
- Remove the legacy `TileGrid` system once the Tilemap workflow is fully confirmed to cover all needs — see [[TileGrid (Legacy System)]]

## Do Not Work On Yet
- Nothing flagged yet.
