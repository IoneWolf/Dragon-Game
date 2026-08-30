# HUD

Both HUD pieces live on the `HUD` GameObject in `Persistent.unity` and are built **entirely in code** at runtime (no `.prefab`/manual UI layout in the Editor). The HUD remains loaded while Main Menu and level content scenes are replaced.

## `HealthBarUI.cs` (`Assets/Scripts/UI/`)
A percentage-based HP bar, top-left of the screen.
- Builds a dark background panel + a colored fill `Image` whose `anchorMax.x` is set to `currentHP / maxHP` (shrinks/grows the bar width to match HP percentage).
- Subscribes to `PlayerHealth.OnHealthChanged` and updates whenever HP changes.
- `GameController` calls `SetTarget(PlayerHealth)` after Level 1 loads and after every level transition. The method safely unsubscribes from the old target and binds the retained Player's health.
- Color thresholds (generalized so it scales with any max HP, not just 3):
  - `> 2/3` HP → **green**
  - `> 1/3` HP → **yellow**
  - `<= 1/3` HP → **red**
- `GetColorForFraction(float)` is a pure static function — unit tested in `Tests/EditMode/HealthBarUITests.cs`.

**Setup:** keep one `HealthBarUI` on the `HUD` GameObject in `Persistent.unity`. Leave **Target** empty; `GameController` assigns it at runtime. Do not add health bars to individual level scenes.

## `GameOverUI.cs` (`Assets/Scripts/UI/`)
A full-screen "GAME OVER" overlay with a Restart button.
- Builds a semi-transparent full-screen `Image` panel, a large bold "GAME OVER" title, and a big Restart button — all sized up significantly (title 120px, button 500x140 with 56px bold label) since the initial pass was too small to read comfortably.
- Starts hidden (`SetActive(false)`); shown when `PlayerHealth.OnPlayerDefeated` fires.
- `GameController` calls `SetTarget(PlayerHealth)` after Level 1 loads and after every level transition, so the persistent overlay always listens to the retained player.
- Sets `Time.timeScale = 0` while shown, pausing all gameplay (movement/physics use scaled time, so everything freezes correctly).
- **Restart** button calls `SceneManager.LoadScene(currentSceneIndex)` and resets `Time.timeScale = 1`.
- Auto-creates an `EventSystem` (using `InputSystemUIInputModule`, matching the project's Input System setup) if one doesn't already exist in the scene — without this, UI buttons silently do nothing when clicked, which was an actual bug encountered during setup.

**Setup:** keep one `GameOverUI` on the Persistent `HUD` GameObject. Leave **Target** empty; `GameController` assigns it at runtime. Do not add game-over overlays to individual level scenes.

## Common Pitfall
If you ever add more UI later and buttons don't respond to clicks, check the Hierarchy for a **duplicate or legacy `EventSystem`** (e.g. one using the old `Standalone Input Module` instead of `Input System UI Input Module`) — only one should exist, and it must use the Input System-compatible module for this project.
