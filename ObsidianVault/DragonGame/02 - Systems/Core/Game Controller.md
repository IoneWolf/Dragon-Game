# Game Controller

`GameController` is the persistent coordinator for scene changes and run state.

## Responsibilities

- Lives only in `Persistent.unity`, which is the first scene in Build Settings.
- Keeps the traveling Player alive with `DontDestroyOnLoad`.
- Loads Main Menu and gameplay content asynchronously with `SceneManager.LoadSceneAsync(..., LoadSceneMode.Additive)`.
- Keeps Persistent loaded, then unloads only the previous content scene after the next one is ready.
- Finds the matching `LevelSpawnPoint`, removes any duplicate destination Player, moves the retained Player, clears physics velocity, and reconnects the destination camera.
- Restarts the active scene in place: the retained Player is restored to full health and moved to its nearest `LevelSpawnPoint` without unloading scene content.
- Prevents duplicate additive loads and temporarily disables `PlayerInput` while a retained Player changes scenes, avoiding duplicate lights and device-pairing conflicts.
- Tracks persistent `score`.

Persistent owns the project's sole `AudioListener` and `GameMusicPlayer`. Levels and Main Menu do not have active audio listeners, so music continues while content scenes are replaced.

Persistent also owns the player HUD (`HealthBarUI` and `GameOverUI`). After Level 1 loads and after every level transition, the controller rebinds both HUD elements to the retained player's `PlayerHealth`, so health and game-over UI remain visible across level changes.

## Inspector Fields

- `levelScenePaths` — full scene paths in travel order, for example:
	- `Assets/Scenes/Feature Sandbox/Level 1.unity`
	- `Assets/Scenes/Feature Sandbox/Level 2.unity`
	- `Assets/Scenes/Feature Sandbox/Level 3.unity`
- `score` — persistent score value.

The controller uses full paths to avoid ambiguous names and fragile Build Settings indices. Only the Persistent-scene controller is authoritative.

## Public Transition Methods

- `LoadLevelByOffset(offset, spawnId, keepPlayerBetweenScenes)` — used by `NextLevel` and `PreviousLevel` exits.
- `LoadSceneByName(sceneName, spawnId, keepPlayerBetweenScenes)` — used by explicit-name exits.
- `LoadSceneByPath(scenePath, spawnId, keepPlayerBetweenScenes)` — preferred code-level load method.
- `StartGame()` — compatibility helper that loads the first scene in `levelScenePaths`.
- `LoadSceneByBuildOffset(...)` and `LoadSceneByBuildIndex(...)` — compatibility helpers; avoid these for new routes.

## Main Menu Routes

`MainMenuUI` uses `LoadSceneByPath()` directly. `Play` starts `Assets/Scenes/Chapter 1/PlayersHouse.unity`; `Playtest` starts `Assets/Scenes/Feature Sandbox/Level 1.unity`. Both paths must remain enabled in Build Settings.

## Camera Arrival Behavior

At a spawn point, `GameController` assigns the destination `CameraFollow` target, snaps the camera to the player, and disables smoothing for the scripted `0.05` second handoff period. Normal camera smoothing resumes afterward.

See [[Level Transitions]] for scene authoring.