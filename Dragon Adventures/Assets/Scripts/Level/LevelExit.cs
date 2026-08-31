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
    public LevelExitMode exitMode = LevelExitMode.NextLevel;
    public string targetSceneName = "Level 1";
    public string targetSpawnId = "Default";
    public bool keepPlayerBetweenScenes = true;

    [Header("Transition Walk")]
    public float walkDirection = 1f;
    public float walkSpeed = 1.5f;
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