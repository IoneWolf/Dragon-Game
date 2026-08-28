using UnityEditor;
using UnityEngine;

// Menu tool: creates a zero-friction PhysicsMaterial2D to prevent characters from snagging
// on seams between adjacent tile colliders when sliding along the ground.
public static class PhysicsMaterialGenerator
{
    private const string FolderPath = "Assets/Physics";
    private const string AssetPath = FolderPath + "/NoFriction.physicsMaterial2D";

    [MenuItem("Tools/Dragon Adventure/Generate No-Friction Physics Material 2D")]
    public static void Generate()
    {
        if (!AssetDatabase.IsValidFolder(FolderPath))
            AssetDatabase.CreateFolder("Assets", "Physics");

        PhysicsMaterial2D existing = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(AssetPath);
        if (existing != null)
        {
            Debug.Log($"No-friction physics material already exists at {AssetPath}.");
            Selection.activeObject = existing;
            return;
        }

        PhysicsMaterial2D material = new PhysicsMaterial2D("NoFriction")
        {
            friction = 0f,
            bounciness = 0f
        };

        AssetDatabase.CreateAsset(material, AssetPath);
        AssetDatabase.SaveAssets();

        Selection.activeObject = material;
        EditorGUIUtility.PingObject(material);
        Debug.Log($"Created {AssetPath}. Assign it to the Tilemap Collider 2D and to the Player/Enemy Rigidbody2D or Collider2D's Material field.");
    }
}
