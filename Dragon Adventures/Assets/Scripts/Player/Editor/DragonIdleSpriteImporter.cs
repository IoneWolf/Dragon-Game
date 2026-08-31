#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class DragonIdleSpriteImporter
{
    private const string DragonIdle1Path = "Assets/Resources/Sprites/DragonIdle1.png";
    private const string DragonIdle2Path = "Assets/Resources/Sprites/DragonIdle2.png";

    [MenuItem("Tools/Dragon Adventure/Configure Dragon Idle Sprites")]
    public static void Configure()
    {
        ConfigureSprite(DragonIdle1Path);
        ConfigureSprite(DragonIdle2Path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ConfigureSprite(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning($"Could not configure missing dragon idle sprite '{assetPath}'.");
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 32f;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
    }
}
#endif
