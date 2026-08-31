# Game Controller

`GameController` is the persistent coordinator for scene changes and run state.

## Responsibilities

- Lives only in `Persistent.unity`, which is the first scene in Build Settings.
- Keeps the traveling Player alive with `DontDestroyOnLoad`.
- Loads Main Menu and gameplay content asynchronously with `SceneManager.LoadSceneAsync(..., LoadSceneMode.Additive)`.
- Keeps Persistent loaded, then unloads only the previous content scene after the next one is ready.
- Finds the matching `LevelSpawnPoint`, removes any duplicate destination Player, moves the retained Player, clears physics velocity, and reconnects the destination camera.
- Tracks persistent `score`.

Persistent owns the project's sole `AudioListener` and `GameMusicPlayer`. Levels and Main Menu do not have active audio listeners, so music continues while content scenes are replaced.

Persistent also owns the player HUD (`HealthBarUI` and `GameOverUI`). After Level 1 loads and after every level transition, the controller rebinds both HUD elements to the retained player's `PlayerHealth`, so health and game-over UI remain visible across level changes.

## Inspector Fields

- `levelScenePaths` — full scene paths in travel order, for example:
	- `Assets/Scenes/Level 1.unity`
	- `Assets/Scenes/Level 2.unity`
	- `Assets/Scenes/Level 3.unity`
- `score` — persistent score value.

The controller uses full paths to avoid ambiguous names and fragile Build Settings indices. Only the Persistent-scene controller is authoritative.

## Public Transition Methods

- `LoadLevelByOffset(offset, spawnId, keepPlayerBetweenScenes)` — used by `NextLevel` and `PreviousLevel` exits.
- `LoadSceneByName(sceneName, spawnId, keepPlayerBetweenScenes)` — used by explicit-name exits.
- `LoadSceneByPath(scenePath, spawnId, keepPlayerBetweenScenes)` — preferred code-level load method.
- `StartGame()` — called by Main Menu Play; replaces Main Menu with Level 1 while Persistent stays loaded.
- `LoadSceneByBuildOffset(...)` and `LoadSceneByBuildIndex(...)` — compatibility helpers; avoid these for new routes.

## Camera Arrival Behavior

At a spawn point, `GameController` assigns the destination `CameraFollow` target, snaps the camera to the player, and disables smoothing for the scripted `0.05` second handoff period. Normal camera smoothing resumes afterward.

See [[Level Transitions]] for scene authoring.