# Asset Requirements

Placeholder-art items that need real art before release.

- **Player sprite** — currently a procedurally generated solid blue square (`PlayerSpriteVisual.cs`). Replace by assigning a real `Sprite` to the `Sprite Renderer`.
- **Enemy sprite** — currently a procedurally generated solid red square (`EnemySpriteVisual.cs`). Replace the same way.
- **Tile art** — the Tilemap currently uses a procedurally generated flat gray tile (`GrayTileGenerator.cs`, Tools → Dragon Adventure → Generate Gray Tile Asset). Replace with real tile art once available.
- **Rock sprite** — currently a procedurally generated black half-circle (`RockVisual.cs`). Replace by assigning a real `Sprite` to the `Sprite Renderer`.
- **Background music** — `GameMusicPlayer.cs` is ready to play startup music from `Assets/Resources/Music`, but the folder still needs real music clips.

See [[Player]], [[Enemy]], [[Grid and Tilemapping]], [[Interaction System]], and [[Music]] for the scripts involved.
