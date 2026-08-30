# Objectives

`ObjectiveTracker` lives in `Persistent.unity`, so completed state remains available while Main Menu and levels are loaded or unloaded. Its panel is hidden in `Persistent` and Main Menu, then appears only when the active content scene is a gameplay level (`Level 1`, `Level 2`, and so on).

## Starting Objectives

- Defeat the red square.
- Talk to the yellow NPC.

The tracker finds the first red `EnemySpriteVisual` and first yellow `NPCVisual` in the active content scene. It marks the objectives complete from `EnemyHealth.OnDefeated` and `NPCInteractable.OnInteracted`.

## Player Controls

- `Tab` expands or minimizes the objective list.

The panel is displayed in the top-left corner during gameplay only. Completed objectives show `[Done]` and become gray.

## Setup

Keep one `Objective Tracker` GameObject in `Persistent.unity`. Do not add tracker copies to levels. For the initial objectives, the target enemy must use a red `EnemySpriteVisual`, and the target NPC must use a yellow `NPCVisual`.

Future objective types should use explicit objective identifiers instead of color matching when the game contains multiple red enemies or yellow NPCs.
