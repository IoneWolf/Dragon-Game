using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// Menu tool: bakes a placeholder gray sprite + Tile asset so you have something to paint
// with in the built-in Tile Palette without needing real art yet.
public static class GrayTileGenerator
{
    private const string FolderPath = "Assets/Tiles";
    private const string TexturePath = FolderPath + "/GrayTileTexture.png";
    private const string TileAssetPath = FolderPath + "/GrayTile.asset";
    private const int TextureSize = 32;

    [MenuItem("Tools/Dragon Adventure/Generate Gray Tile Asset")]
    public static void Generate()
    {
        if (!AssetDatabase.IsValidFolder(FolderPath))
            AssetDatabase.CreateFolder("Assets", "Tiles");

        WriteGrayTexture();
        AssetDatabase.ImportAsset(TexturePath);
        ConfigureAsSprite();

        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(TexturePath);
        CreateTileAsset(sprite);

        AssetDatabase.SaveAssets();
        Debug.Log($"Gray tile asset created at {TileAssetPath}. Open Window > 2D > Tile Palette, " +
                   "create a palette, and drag this asset in to start painting.");
    }

    private static void WriteGrayTexture()
    {
        Texture2D texture = new Texture2D(TextureSize, TextureSize);
        Color[] pixels = new Color[TextureSize * TextureSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.gray;
        texture.SetPixels(pixels);
        texture.Apply();
        File.WriteAllBytes(TexturePath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
    }

    private static void ConfigureAsSprite()
    {
        TextureImporter importer = AssetImporter.GetAtPath(TexturePath) as TextureImporter;
        if (importer == null) return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = TextureSize;
        importer.filterMode = FilterMode.Point;
        importer.SaveAndReimport();
    }

    private static void CreateTileAsset(Sprite sprite)
    {
        Tile existing = AssetDatabase.LoadAssetAtPath<Tile>(TileAssetPath);
        Tile tile = existing != null ? existing : ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.color = Color.white;

        if (existing == null)
            AssetDatabase.CreateAsset(tile, TileAssetPath);
        else
            EditorUtility.SetDirty(tile);

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tile;
        EditorGUIUtility.PingObject(tile);
    }
}
