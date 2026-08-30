# Game Controller

`GameController` is the persistent coordinator for scene changes and run state.

## Responsibilities

- Lives on a `GameController` GameObject in each gameplay scene; duplicates destroy themselves, leaving one persistent controller.
- Keeps the traveling Player alive with `DontDestroyOnLoad`.
- Loads destinations asynchronously with `SceneManager.LoadSceneAsync(..., LoadSceneMode.Single)`.
- Stores the pending destination path and spawn ID until Unity raises `sceneLoaded`.
- Finds the matching `LevelSpawnPoint`, removes any duplicate destination Player, moves the retained Player, clears physics velocity, and reconnects the destination camera.
- Tracks persistent `score`.

`Single` scene loading is intentional: it guarantees the old scene's camera and `AudioListener` are unloaded before the new scene is active. Each level should therefore contain exactly one Main Camera with exactly one AudioListener.

## Inspector Fields

- `levelScenePaths` — full scene paths in travel order, for example:
	- `Assets/Scenes/Level 1.unity`
	- `Assets/Scenes/Level 2.unity`
	- `Assets/Scenes/Level 3.unity`
- `score` — persistent score value.

The controller uses full paths to avoid ambiguous names and fragile Build Settings indices. These paths must be present on every scene-local GameController because the first one loaded persists for the rest of the play session.

## Public Transition Methods

- `LoadLevelByOffset(offset, spawnId, keepPlayerBetweenScenes)` — used by `NextLevel` and `PreviousLevel` exits.
- `LoadSceneByName(sceneName, spawnId, keepPlayerBetweenScenes)` — used by explicit-name exits.
- `LoadSceneByPath(scenePath, spawnId, keepPlayerBetweenScenes)` — preferred code-level load method.
- `LoadSceneByBuildOffset(...)` and `LoadSceneByBuildIndex(...)` — compatibility helpers; avoid these for new routes.

## Camera Arrival Behavior

At a spawn point, `GameController` assigns the destination `CameraFollow` target, snaps the camera to the player, and disables smoothing for the scripted `0.05` second handoff period. Normal camera smoothing resumes afterward.

See [[Level Transitions]] for scene authoring.