using NUnit.Framework;
using UnityEngine;

// Unit tests for TileGrid. On failure the Test Runner console shows this class/file and line.
public class TileGridTests
{
    private GameObject go;
    private TileGrid grid;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("TestGrid");
        grid = go.AddComponent<TileGrid>();
        grid.width = 4;
        grid.height = 4;
        grid.EnsureMapSize();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    [Test]
    public void EnsureMapSize_CreatesCorrectLength()
    {
        Assert.AreEqual(16, grid.tiles.Length,
            $"[{nameof(TileGridTests)}] Scripts/Tiles/TileGrid.cs: tiles array length is wrong for a 4x4 grid.");
    }

    [Test]
    public void SetTile_ThenGetTile_ReturnsSameValue()
    {
        grid.SetTile(1, 2, true);
        Assert.IsTrue(grid.GetTile(1, 2),
            $"[{nameof(TileGridTests)}] Scripts/Tiles/TileGrid.cs: SetTile(1,2,true) did not persist in GetTile.");
    }

    [Test]
    public void ToggleTile_FlipsValue()
    {
        bool before = grid.GetTile(0, 0);
        grid.ToggleTile(0, 0);
        Assert.AreEqual(!before, grid.GetTile(0, 0),
            $"[{nameof(TileGridTests)}] Scripts/Tiles/TileGrid.cs: ToggleTile(0,0) did not flip the tile state.");
    }

    [Test]
    public void GetTile_OutOfBounds_ReturnsFalse()
    {
        Assert.IsFalse(grid.GetTile(-1, 0),
            $"[{nameof(TileGridTests)}] Scripts/Tiles/TileGrid.cs: GetTile should return false for out-of-range x.");
        Assert.IsFalse(grid.GetTile(0, 99),
            $"[{nameof(TileGridTests)}] Scripts/Tiles/TileGrid.cs: GetTile should return false for out-of-range y.");
    }

    [Test]
    public void FillAll_True_BuildsExpectedTileCount()
    {
        grid.FillAll(true);
        Transform tilesParent = go.transform.Find("Tiles");
        Assert.IsNotNull(tilesParent,
            $"[{nameof(TileGridTests)}] Scripts/Tiles/TileGrid.cs: 'Tiles' parent was not created by FillAll.");
        Assert.AreEqual(grid.width * grid.height, tilesParent.childCount,
            $"[{nameof(TileGridTests)}] Scripts/Tiles/TileGrid.cs: expected {grid.width * grid.height} cube tiles after FillAll(true).");
    }

    [Test]
    public void ClearGrid_RemovesAllTiles()
    {
        grid.FillAll(true);
        grid.ClearGrid();
        Transform tilesParent = go.transform.Find("Tiles");
        Assert.AreEqual(0, tilesParent.childCount,
            $"[{nameof(TileGridTests)}] Scripts/Tiles/TileGrid.cs: ClearGrid left {tilesParent.childCount} tiles behind.");
    }
}
