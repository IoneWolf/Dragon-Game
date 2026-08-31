using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelExitMode
{
    NextLevel,
    PreviousLevel,
    ExplicitSceneName
}

// Proximity-based scene exit that walks the player into the transition before loading.
[AddComponentMenu("Dragon Adventure/Level/Level Exit")]
public class LevelExit : MonoBehaviour
{
    [Tooltip("How to choose the destination scene: next/previous entry in GameController, or an explicit scene name.")]
    public LevelExitMode exitMode = LevelExitMode.NextLevel;
    [Tooltip("Destination scene name for Explicit Scene Name mode. It must match an entry in GameController's level list.")]
    public string targetSceneName = "Level 1";
    [Tooltip("Spawn ID on the destination LevelSpawnPoint where the player should appear.")]
    public string targetSpawnId = "Default";
    [Tooltip("Keep the current Player object and move it to the destination scene's spawn point.")]
    public bool keepPlayerBetweenScenes = true;

    [Header("Transition Walk")]
    [Tooltip("Horizontal walk direction for Explicit Scene Name exits: positive is right, negative is left. Ignored by next/previous exits.")]
    public float walkDirection = 1f;
    [Tooltip("Forced walking speed in world units per second before the destination scene loads.")]
    public float walkSpeed = 1.5f;
    [Tooltip("Seconds the player walks into the exit before the destination scene starts loading.")]
    public float walkDuration = 0.6f;

    private bool transitionStarted;

    private void Awake()
    {
        EnsureTriggerCollider();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transitionStarted) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            StartCoroutine(TransitionRoutine(player));
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

    private IEnumerator TransitionRoutine(PlayerController player)
    {
        transitionStarted = true;
        player.StartScriptedMovement(GetWalkDirection(), walkSpeed);
        yield return new WaitForSeconds(walkDuration);
        player.StopScriptedMovement();

        GameController controller = GameController.Instance;
        if (controller == null)
        {
            Debug.LogWarning($"{nameof(LevelExit)} on {name} needs a GameController in the scene.");
            transitionStarted = false;
            yield break;
        }

        bool loadStarted = exitMode == LevelExitMode.ExplicitSceneName
            ? controller.LoadSceneByName(targetSceneName, targetSpawnId, keepPlayerBetweenScenes)
            : controller.LoadLevelByOffset(GetLevelOffset(), targetSpawnId, keepPlayerBetweenScenes);

        if (!loadStarted)
        {
            Debug.LogWarning($"{nameof(LevelExit)} on {name} could not resolve a target scene.");
            transitionStarted = false;
        }
    }

    private int GetLevelOffset()
    {
        return exitMode == LevelExitMode.NextLevel ? 1 : -1;
    }

    private float GetWalkDirection()
    {
        return exitMode == LevelExitMode.PreviousLevel ? -1f : walkDirection;
    }
}