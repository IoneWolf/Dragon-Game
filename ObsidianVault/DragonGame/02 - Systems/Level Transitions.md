# Level Transitions

Interact with a `LevelExit` using `E` to move between levels. `GameController` loads the destination asynchronously, keeps the player alive, then places that same player at the requested `LevelSpawnPoint`.

## Level Chain

The current ordered route is:

`Level 1 -> Level 2 -> Level 3`

The `levelScenePaths` list on the `GameController` in `Persistent.unity` must use that exact order. Every level must also be enabled in **File -> Build Profiles / Build Settings**.

## Spawn Point Rule

`LevelSpawnPoint.prefab` is the one reusable template. Drag it into a scene, position the instance, rename it for clarity, and set its `spawnId`.

- `FromLeft` means the player arrived from the level on the left. Place it at this level's left entrance.
- `FromRight` means the player arrived from the level on the right. Place it at this level's right entrance.

The spawn ID on the destination point must exactly match the exit's `targetSpawnId`.

| Current scene | Exit location | `exitMode` | Destination spawn ID |
| --- | --- | --- | --- |
| Level 1 | Right | `NextLevel` | `FromLeft` in Level 2 |
| Level 2 | Left | `PreviousLevel` | `FromRight` in Level 1 |
| Level 2 | Right | `NextLevel` | `FromLeft` in Level 3 |
| Level 3 | Left | `PreviousLevel` | `FromRight` in Level 2 |

For a new Level 4, add it after Level 3 in `levelScenePaths` and Build Settings. Add a Level 3 right exit set to `NextLevel` / `FromLeft`, then a Level 4 left spawn with ID `FromLeft`. Add the reverse Level 4 left exit as `PreviousLevel` / `FromRight`, and a Level 3 right spawn with ID `FromRight`.

## Scripts And Prefabs

### `LevelExit.cs` (`Assets/Scripts/Level/`)

- Put this on the door, edge, or portal the player interacts with.
- Requires `InteractionPromptIcon` and supplies a trigger `BoxCollider2D` when absent.
- `NextLevel` and `PreviousLevel` follow the ordered `levelScenePaths` list.
- `ExplicitSceneName` uses `targetSceneName` when the route is not part of the linear chain.
- Set `targetSpawnId` to the ID on the destination `LevelSpawnPoint`.
- Keep `keepPlayerBetweenScenes` enabled for normal gameplay.

### `LevelSpawnPoint.cs` (`Assets/Scripts/Level/`)

- Use an instance of `LevelSpawnPoint.prefab`, not a separate left/right prefab.
- Shows a cyan gameplay marker and green Scene-view gizmo.
- `connectMainCameraToPlayer` reconnects the destination `Main Camera` to the retained player.
- `cameraOffset` controls that camera's framing after arrival.

### `GameController.cs` (`Assets/Scripts/Game/`)

- Stores the pending scene path and spawn ID.
- Uses `SceneManager.LoadSceneAsync` with `LoadSceneMode.Additive`, then unloads the previous content scene after the destination is ready.
- Keeps the traveling Player using `DontDestroyOnLoad`.
- Removes any Player instantiated by the destination scene, then moves the retained player to the matched spawn and clears its velocity.
- A missing spawn ID produces a warning instead of silently using the Player prefab position.

## Level Authoring Checklist

1. Add the scene to Build Settings.
2. Add its full path to `Persistent`'s `GameController.levelScenePaths` list in travel order.
3. Add one scene-owned `Main Camera`, with no active `AudioListener`.
4. Do not add HUD, music, or `GameController` objects to the level; `Persistent.unity` owns those systems.
5. Drag in `LevelSpawnPoint.prefab` for each possible entrance and assign unique direction-appropriate IDs.
6. Add `LevelExit` instances for each possible departure and configure their mode and destination spawn ID.
7. Test forward and reverse travel by starting from Persistent. Confirm the player arrives at the marker, the health HUD remains visible, only one Player and one active camera exist, music continues, and the Console has no missing-spawn or audio-listener warnings.

## Current Limits

- No fade or loading-screen UI.
- The route is a linear ordered list; use `ExplicitSceneName` for branches or portals.
- Inventory, quest state, and checkpoints are not yet tracked by transitions.