# Interaction System

Lets the player press **Interact** (`E`) to trigger nearby world objects, via a small interface-based contract so any object can opt in without the player needing to know about specific types.

## Scripts

### `IInteractable.cs` (`Assets/Scripts/Common/`)
A single-method interface: `void Interact()`. Any `MonoBehaviour` that implements it can be triggered by the player.

### `PlayerInputHandler.cs` (`Assets/Scripts/Player/`)
- Exposes an `InteractPressed` event, fired from `OnInteract(InputValue)` when the `Interact` action (bound to `E` / gamepad North button in `InputSystem_Actions`) is pressed.
- The `Interact` action originally had a **Hold** interaction (Unity's default template value) — changed to a plain press so a single tap triggers it.

### `PlayerInteractor.cs` (`Assets/Scripts/Player/`)
- Subscribes to `PlayerInputHandler.InteractPressed`.
- On press, runs a `Physics2D.OverlapCircleNonAlloc` (`interactRadius`, default 1.5) around the player, collects every `IInteractable` found (via `GetComponentInParent`, so the interface can live on a parent object), and calls `Interact()` on whichever is closest.
- Ignores the press entirely while `DialogueUI.IsOpen` is true, so Interact can't re-trigger a new interactable (or the same one) while a conversation is already on screen.
- Draws a cyan gizmo sphere of `interactRadius` when selected, for tuning range in the Scene view.

## Example Interactable: Rock
`RockVisual.cs` + `RockInteractable.cs` (`Assets/Scripts/Interactables/`) — a simple talking rock, split the same way `Player`/`Enemy` split their sprite-generation component from their logic component:
- `RockVisual` (`[ExecuteAlways]`) generates a black half-circle placeholder sprite via `HalfCircleSpriteFactory` (`Assets/Scripts/Common/`), visible in the Scene view without pressing Play.
- `RockInteractable` auto-adds a trigger `CircleCollider2D` in `Awake()` if one isn't already present (no manual collider setup needed), and drives the dialogue — see [[Dialogue System]] for the actual conversation.

## Example Interactable: NPC
`NPCVisual.cs` + `NPCInteractable.cs` (`Assets/Scripts/NPC/`) — a yellow square placeholder NPC that uses `NPCDialogueData` assets for editable dialogue content. See [[NPC]].

## Example Interactable: Level Exit
`LevelExit.cs` (`Assets/Scripts/Level/`) — an interactable area/door/edge trigger that loads another scene and requests a matching `LevelSpawnPoint`. See [[Level Transitions]].

## Scene Setup
- `Player` GameObject needs a `PlayerInteractor` component added (alongside the components listed in [[Player]]).
- Any interactable object needs a `Collider2D` (auto-added by `RockInteractable` if missing) and a component implementing `IInteractable`.
- NPC objects should also have `InteractionPromptIcon` so the player sees the existing "Press E" prompt while in range.

## Dependencies
- [[Dialogue System]] — interactables commonly use it to show their response.
- [[Player]] — `PlayerInteractor` requires `PlayerInputHandler`.
- [[Level Transitions]] — exits use the same interactable path to move between areas.

## Status
Working.
