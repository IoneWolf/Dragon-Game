# Music

## `GameMusicPlayer.cs` (`Assets/Scripts/Audio/`)

Startup/background music system.

- Lives on the `GameMusicPlayer` GameObject in `Persistent.unity`.
- Uses an `AudioSource` for playback; the component is required and configured for 2D music.
- Persistent owns the scene itself, so music continues while Main Menu and level content scenes are replaced.
- If `startupMusic` is assigned in the Inspector, that clip plays on start.
- If `startupMusic` is empty and `loadFirstResourcesClip` is enabled, it first tries `Assets/Resources/Music/Soundtrack derg game.mp3`, then falls back to the first `AudioClip` found in `Assets/Resources/Music`.
- Logs `Playing music: <clip name>` when playback starts, or a warning if no clip is found.

## Scene Setup

Keep one `GameMusicPlayer` GameObject in `Persistent.unity` with an `AudioSource` and `GameMusicPlayer`. Do not add music players to Main Menu or level scenes. Persistent also owns the sole active `AudioListener`.

Leave `startupMusic` empty to use the configured Resources path, or assign `Soundtrack derg game.mp3` directly.

If the object exists but no music is heard, check:
- Game view audio mute is off.
- The active camera has an `AudioListener`.
- The `AudioSource` and `GameMusicPlayer` components are enabled.
- `volume` is above `0`.

## Music Folder

Put music files in `Assets/Resources/Music`.

Supported Unity audio formats include `.wav`, `.mp3`, and `.ogg`.

Current startup/Level 1 track: `Assets/Resources/Music/Soundtrack derg game.mp3`.

For quick setup, drop one music clip into that folder, configure the Persistent music object, and start Play Mode from `Persistent.unity`.

For manual control, configure `startupMusic`, `volume`, and `loop` on the Persistent `GameMusicPlayer`.