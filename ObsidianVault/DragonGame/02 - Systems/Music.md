# Music

## `GameMusicPlayer.cs` (`Assets/Scripts/Audio/`)

Startup/background music system.

- Lives on a normal scene GameObject named `Game Music Player`.
- Uses an `AudioSource` for playback; the component is required and configured for 2D music.
- Persists between scenes by default so music does not restart on scene changes.
- If `startupMusic` is assigned in the Inspector, that clip plays on start.
- If `startupMusic` is empty and `loadFirstResourcesClip` is enabled, it first tries `Assets/Resources/Music/Soundtrack derg game.mp3`, then falls back to the first `AudioClip` found in `Assets/Resources/Music`.
- Logs `Playing music: <clip name>` when playback starts, or a warning if no clip is found.

## Scene Setup

Create a normal scene GameObject named `GameMusicPlayer` or `Game Music Player`.

Add these components:
- `AudioSource`
- `GameMusicPlayer`

Assign `Soundtrack derg game.mp3` to the `startupMusic` field, then save the scene so the music object stays in `Level 1.unity`.

`Level 1.unity` currently has a `GameMusicPlayer` object with an `AudioSource` using `Soundtrack derg game.mp3`.

If the object exists but no music is heard, check:
- Game view audio mute is off.
- The active camera has an `AudioListener`.
- The `AudioSource` and `GameMusicPlayer` components are enabled.
- `volume` is above `0`.

## Music Folder

Put music files in `Assets/Resources/Music`.

Supported Unity audio formats include `.wav`, `.mp3`, and `.ogg`.

Current startup/Level 1 track: `Assets/Resources/Music/Soundtrack derg game.mp3`.

For quick setup, drop one music clip into that folder, create the `GameMusicPlayer` scene object, assign the clip, save the scene, and press Play.

For manual control, add `GameMusicPlayer` to a scene GameObject and assign `startupMusic`, `volume`, `loop`, and `persistBetweenScenes` in the Inspector.