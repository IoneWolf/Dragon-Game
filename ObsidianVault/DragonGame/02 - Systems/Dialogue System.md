# Dialogue System

A simple runtime-built dialogue box: a speaker sprite, a line of text, and optional choice buttons. Used by interactables (see [[Interaction System]]) to talk to the player.

## Scripts (`Assets/Scripts/Dialogue/`)

### `DialogueChoice.cs`
A tiny data class: `Text` (button label) + `OnChosen` (an `Action` run when the player clicks it).

### `NPCDialogueData.cs`
A `ScriptableObject` asset used by NPCs so dialogue content can be edited separately from interaction logic. Create one via **Assets -> Create -> Dragon Adventure -> Dialogue -> NPC Dialogue**.

### `DialogueUI.cs`
- Static API: `DialogueUI.Show(sprite, text, choices)` and `DialogueUI.Hide()`. `DialogueUI.IsOpen` reports whether the panel is currently visible.
- Lazily creates its own hidden singleton `GameObject` the first time `Show` is called — callers never need to place it in the scene.
- Builds its own `Canvas`/`EventSystem` if the scene doesn't already have one, the same pattern `GameOverUI`/`HealthBarUI` use.
- Layout: a panel stretched across the **full width of the screen, anchored to the bottom** (300px tall) — speaker sprite on the left, body text top-right, choice buttons stacked below the text (`VerticalLayoutGroup`).
- Re-showing replaces the current line/choices and destroys the previous choice buttons, so callers can chain `Show` calls to build a short conversation (see `RockInteractable.Interact()` → `HandlePickUp()`).

## Dependencies
- [[Player]] — `PlayerController` checks `DialogueUI.IsOpen` in `FixedUpdate`/`Update` to freeze horizontal movement, jumping, and sprite-facing updates while a conversation is open (gravity keeps acting normally, so an airborne player still lands).
- [[Interaction System]] — interactables are the callers of `DialogueUI.Show`.
- [[NPC]] — NPCs use `NPCDialogueData` assets and display them through `DialogueUI`.

## Status
Working.
