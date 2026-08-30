using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelExitMode
{
    NextLevel,
    PreviousLevel,
    ExplicitSceneName
}

// Interactable scene exit: press Interact near it to load another scene and request a spawn point.
[AddComponentMenu("Dragon Adventure/Level/Level Exit")]
[RequireComponent(typeof(InteractionPromptIcon))]
public class LevelExit : MonoBehaviour, IInteractable
{
    public LevelExitMode exitMode = LevelExitMode.NextLevel;
    public string targetSceneName = "Level 1";
    public string targetSpawnId = "Default";
    public string promptText = "Press E to proceed";
    public bool keepPlayerBetweenScenes = true;

    private void Awake()
    {
        GetComponent<InteractionPromptIcon>().SetPromptText(promptText);
        EnsureTriggerCollider();
    }

    private void EnsureTriggerCollider()
    {
        Collider2D existing = GetComponent<Collider2D>();
        if (existing != null)
        {
            existing.isTrigger = true;
            return;
        }

        BoxCollider2D box = gameObject.AddComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.size = Vector2.one;
    }

    public void Interact()
    {
        GameController controller = GameController.Instance;
        if (controller == null)
        {
            Debug.LogWarning($"{nameof(LevelExit)} on {name} needs a GameController in the scene.");
            return;
        }

        bool loadStarted = exitMode == LevelExitMode.ExplicitSceneName
            ? controller.LoadSceneByName(targetSceneName, targetSpawnId, keepPlayerBetweenScenes)
            : controller.LoadLevelByOffset(GetLevelOffset(), targetSpawnId, keepPlayerBetweenScenes);

        if (!loadStarted)
        {
            Debug.LogWarning($"{nameof(LevelExit)} on {name} could not resolve a target scene.");
        }
    }

    private int GetLevelOffset()
    {
        return exitMode == LevelExitMode.NextLevel ? 1 : -1;
    }
}