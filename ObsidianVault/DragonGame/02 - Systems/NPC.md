# NPC

Interactable non-player character system for simple conversations. The current NPC is a yellow square placeholder with editable dialogue stored outside the interaction script.

## Functionality

- The NPC appears as a yellow square placeholder in both Edit mode and Play mode.
- When the player enters interaction range, `PlayerInteractor` can show the NPC's `InteractionPromptIcon` prompt.
- Pressing Interact calls `NPCInteractable.Interact()` through the existing `IInteractable` interface.
- `NPCInteractable` opens `DialogueUI` with an opening line and a list of choices from `NPCDialogueData`.
- Selecting a normal choice shows that option's response text, with buttons to go `Back` to the opening choices or say `Goodbye.`.
- Selecting a choice marked `closesDialogue` closes the dialogue immediately.
- If no dialogue data asset is assigned, the NPC still works with a fallback `Hey there.` line and a `Goodbye.` button.

## Scripts

### `NPCVisual.cs` (`Assets/Scripts/NPC/`)
- Generates a yellow square placeholder sprite using `SquareSpriteFactory`.
- Runs in edit mode so the NPC is visible in the Scene view before Play.
- Exposes `spriteColor` and `size` for quick placeholder tuning.

### `NPCInteractable.cs` (`Assets/Scripts/NPC/`)
- Implements `IInteractable`, so `PlayerInteractor` can trigger the NPC with the existing Interact input.
- Requires `NPCVisual` and `InteractionPromptIcon`.
- Ensures the NPC has a trigger `BoxCollider2D` if no collider exists.
- Opens `DialogueUI` using an assigned `NPCDialogueData` asset.
- If no dialogue asset is assigned, falls back to a short default greeting with a Goodbye choice.

### `NPCDialogueData.cs` (`Assets/Scripts/Dialogue/`)
Unity `ScriptableObject` dialogue asset for NPC content.

Create one via **Assets -> Create -> Dragon Adventure -> Dialogue -> NPC Dialogue**.

Starter asset: `Assets/Dialogue/Friendly NPC Dialogue.asset`.

Fields:
- `speakerSprite` — optional portrait/speaker image shown in the dialogue UI.
- `openingLine` — first line shown when interacting with the NPC.
- `options` — list of choices. Each option has `choiceText`, `responseText`, and `closesDialogue`.

The goal is to keep writing/content changes in data assets instead of hardcoding every NPC conversation in `NPCInteractable`.

## Scene Setup

To add an NPC to a level:
1. Create an empty GameObject named something like `NPC`.
2. Add `SpriteRenderer`.
3. Add `NPCVisual`.
4. Add `NPCInteractable`.
5. Put the GameObject on the interactable layer used by `PlayerInteractor`.
6. Assign `Assets/Dialogue/Friendly NPC Dialogue.asset` to `NPCInteractable.dialogueData`, or create another `NPCDialogueData` asset for different dialogue.

## Status
Initial placeholder NPC implemented: yellow square visual, prompt support, and editable dialogue data.

## Known Limits
- Dialogue choices currently support one response level plus `Back` / `Goodbye.`.
- There is no branching state, quest flag, or conditional dialogue yet.
- No NPC prefab has been committed yet; scene setup is currently manual.