using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[AddComponentMenu("Dragon Adventure/UI/Objective Tracker")]
public class ObjectiveTracker : MonoBehaviour
{
    private const float ToggleDuration = 0.2f;
    private static readonly Vector2 ExpandedPanelSize = new Vector2(990f, 360f);
    private static readonly Vector2 CollapsedPanelSize = new Vector2(990f, 126f);

    [Tooltip("Text shown for the objective completed when the tracked enemy is defeated.")]
    public string defeatObjectiveText = "Defeat the red square";
    [Tooltip("Text shown for the objective completed when the tracked NPC is spoken to.")]
    public string talkObjectiveText = "Talk to the yellow NPC";
    [Tooltip("Pixel offset from the top-left corner of the screen.")]
    public Vector2 screenOffset = new Vector2(20f, -60f);

    private EnemyHealth trackedEnemy;
    private NPCInteractable trackedNpc;
    private Text defeatObjectiveLabel;
    private Text talkObjectiveLabel;
    private GameObject objectivePanel;
    private GameObject objectiveList;
    private RectTransform objectivePanelRect;
    private bool defeatObjectiveComplete;
    private bool talkObjectiveComplete;
    private bool isExpanded = true;
    private Coroutine toggleRoutine;

    private void Awake()
    {
        BuildUI();
        UpdateVisibility(SceneManager.GetActiveScene());
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += HandleActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= HandleActiveSceneChanged;
        UnsubscribeTargets();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isExpanded = !isExpanded;
            if (toggleRoutine != null)
                StopCoroutine(toggleRoutine);

            toggleRoutine = StartCoroutine(AnimatePanelSize());
        }
    }

    private void HandleActiveSceneChanged(Scene previousScene, Scene nextScene)
    {
        UpdateVisibility(nextScene);

        if (!IsGameplayScene(nextScene))
        {
            UnsubscribeTargets();
            return;
        }

        BindTargetsInActiveScene();
    }

    private void UpdateVisibility(Scene scene)
    {
        objectivePanel.SetActive(IsGameplayScene(scene));
    }

    private void BindTargetsInActiveScene()
    {
        UnsubscribeTargets();

        foreach (EnemySpriteVisual enemyVisual in FindObjectsByType<EnemySpriteVisual>(FindObjectsSortMode.None))
        {
            if (enemyVisual.gameObject.scene != SceneManager.GetActiveScene() || !IsRed(enemyVisual.spriteColor))
                continue;

            trackedEnemy = enemyVisual.GetComponent<EnemyHealth>();
            if (trackedEnemy != null && !defeatObjectiveComplete)
                trackedEnemy.OnDefeated += CompleteDefeatObjective;
            break;
        }

        foreach (NPCVisual npcVisual in FindObjectsByType<NPCVisual>(FindObjectsSortMode.None))
        {
            if (npcVisual.gameObject.scene != SceneManager.GetActiveScene() || !IsYellow(npcVisual.spriteColor))
                continue;

            trackedNpc = npcVisual.GetComponent<NPCInteractable>();
            if (trackedNpc != null && !talkObjectiveComplete)
                trackedNpc.OnInteracted += CompleteTalkObjective;
            break;
        }
    }

    private void UnsubscribeTargets()
    {
        if (trackedEnemy != null)
            trackedEnemy.OnDefeated -= CompleteDefeatObjective;
        if (trackedNpc != null)
            trackedNpc.OnInteracted -= CompleteTalkObjective;

        trackedEnemy = null;
        trackedNpc = null;
    }

    private void CompleteDefeatObjective()
    {
        defeatObjectiveComplete = true;
        if (trackedEnemy != null)
            trackedEnemy.OnDefeated -= CompleteDefeatObjective;
        UpdateLabels();
    }

    private void CompleteTalkObjective()
    {
        talkObjectiveComplete = true;
        if (trackedNpc != null)
            trackedNpc.OnInteracted -= CompleteTalkObjective;
        UpdateLabels();
    }

    private void BuildUI()
    {
        GameObject canvasObject = new GameObject("Objective Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        objectivePanel = new GameObject("Objective Panel", typeof(Image));
        objectivePanel.transform.SetParent(canvas.transform, false);
        objectivePanelRect = objectivePanel.GetComponent<RectTransform>();
        objectivePanelRect.anchorMin = new Vector2(0f, 1f);
        objectivePanelRect.anchorMax = new Vector2(0f, 1f);
        objectivePanelRect.pivot = new Vector2(0f, 1f);
        objectivePanelRect.anchoredPosition = screenOffset;
        objectivePanelRect.sizeDelta = ExpandedPanelSize;
        objectivePanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.65f);

        CreateLabel(objectivePanel.transform, "Objectives", new Vector2(36f, -24f), 78, FontStyle.Bold, Color.white);

        objectiveList = new GameObject("Objective List");
        objectiveList.transform.SetParent(objectivePanel.transform, false);
        RectTransform listRect = objectiveList.AddComponent<RectTransform>();
        listRect.anchorMin = new Vector2(0f, 1f);
        listRect.anchorMax = new Vector2(1f, 1f);
        listRect.pivot = new Vector2(0.5f, 1f);
        listRect.anchoredPosition = new Vector2(0f, -126f);
        listRect.sizeDelta = new Vector2(0f, 222f);

        defeatObjectiveLabel = CreateLabel(objectiveList.transform, string.Empty, new Vector2(36f, -6f), 60, FontStyle.Normal, Color.white);
        talkObjectiveLabel = CreateLabel(objectiveList.transform, string.Empty, new Vector2(36f, -114f), 60, FontStyle.Normal, Color.white);
        UpdateLabels();
    }

    private static Text CreateLabel(Transform parent, string value, Vector2 position, int fontSize, FontStyle fontStyle, Color color)
    {
        GameObject labelObject = new GameObject("Label", typeof(Text));
        labelObject.transform.SetParent(parent, false);
        Text label = labelObject.GetComponent<Text>();
        label.text = value;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.color = color;
        label.alignment = TextAnchor.MiddleLeft;

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 1f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.pivot = new Vector2(0f, 1f);
        labelRect.anchoredPosition = position;
        labelRect.sizeDelta = new Vector2(-72f, 96f);
        return label;
    }

    private IEnumerator AnimatePanelSize()
    {
        if (isExpanded)
            objectiveList.SetActive(true);

        Vector2 startingSize = objectivePanelRect.sizeDelta;
        Vector2 targetSize = isExpanded ? ExpandedPanelSize : CollapsedPanelSize;
        float elapsed = 0f;

        while (elapsed < ToggleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            objectivePanelRect.sizeDelta = Vector2.Lerp(startingSize, targetSize, Mathf.Clamp01(elapsed / ToggleDuration));
            yield return null;
        }

        objectivePanelRect.sizeDelta = targetSize;
        if (!isExpanded)
            objectiveList.SetActive(false);

        toggleRoutine = null;
    }

    private void UpdateLabels()
    {
        defeatObjectiveLabel.text = FormatObjective(defeatObjectiveText, defeatObjectiveComplete);
        defeatObjectiveLabel.color = defeatObjectiveComplete ? Color.gray : Color.white;
        talkObjectiveLabel.text = FormatObjective(talkObjectiveText, talkObjectiveComplete);
        talkObjectiveLabel.color = talkObjectiveComplete ? Color.gray : Color.white;
    }

    private static string FormatObjective(string objectiveText, bool complete)
    {
        return complete ? "[Done] " + objectiveText : "[ ] " + objectiveText;
    }

    private static bool IsRed(Color color)
    {
        return color.r > 0.8f && color.g < 0.3f && color.b < 0.3f;
    }

    private static bool IsYellow(Color color)
    {
        return color.r > 0.8f && color.g > 0.8f && color.b < 0.3f;
    }

    private static bool IsGameplayScene(Scene scene)
    {
        return scene.path.StartsWith("Assets/Scenes/Level ");
    }
}
