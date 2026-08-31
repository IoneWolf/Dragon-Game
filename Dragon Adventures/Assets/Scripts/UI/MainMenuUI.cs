using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Builds a simple title/Play/Quit main menu entirely in code, same approach as HealthBarUI/GameOverUI.
public class MainMenuUI : MonoBehaviour
{
    [Tooltip("Text displayed as the main menu title.")]
    public string gameTitle = "Dragon Adventure";
    [Tooltip("Full Build Settings scene path loaded by the Play button.")]
    public string playScenePath = "Assets/Scenes/Chapter 1/PlayersHouse.unity";
    [Tooltip("Full Build Settings scene path loaded by the Playtest button.")]
    public string playtestScenePath = "Assets/Scenes/Feature Sandbox/Level 1.unity";

    private void Awake()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        EnsureEventSystem();

        GameObject canvasObject = new GameObject("Menu Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject background = new GameObject("Background", typeof(Image));
        background.transform.SetParent(canvas.transform, false);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        background.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 1f);

        BuildTitle(canvas.transform);
        BuildButton(canvas.transform, "Play", new Vector2(0.5f, 0.45f), PlayGame);
        BuildButton(canvas.transform, "Playtest", new Vector2(0.5f, 0.3f), Playtest);
        BuildButton(canvas.transform, "Quit", new Vector2(0.5f, 0.15f), QuitGame);
    }

    private void BuildTitle(Transform parent)
    {
        GameObject titleObject = new GameObject("Title", typeof(Text));
        titleObject.transform.SetParent(parent, false);

        Text title = titleObject.GetComponent<Text>();
        title.text = gameTitle;
        title.alignment = TextAnchor.MiddleCenter;
        title.fontSize = 90;
        title.fontStyle = FontStyle.Bold;
        title.color = Color.white;
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.65f);
        titleRect.anchorMax = new Vector2(0.5f, 0.65f);
        titleRect.sizeDelta = new Vector2(900f, 140f);
        titleRect.anchoredPosition = Vector2.zero;
    }

    private void BuildButton(Transform parent, string label, Vector2 anchor, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchor;
        buttonRect.anchorMax = anchor;
        buttonRect.sizeDelta = new Vector2(400f, 100f);
        buttonRect.anchoredPosition = Vector2.zero;

        buttonObject.GetComponent<Image>().color = Color.white;
        buttonObject.GetComponent<Button>().onClick.AddListener(onClick);

        GameObject labelObject = new GameObject("Text", typeof(Text));
        labelObject.transform.SetParent(buttonObject.transform, false);

        Text text = labelObject.GetComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 40;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
    }

    // Without an EventSystem in the scene, UI clicks are silently ignored.
    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    public void PlayGame()
    {
        GameController controller = GameController.Instance;
        if (controller == null)
        {
            Debug.LogWarning("Main Menu needs a persistent GameController to start the game.");
            return;
        }

        controller.LoadSceneByPath(playScenePath, null, false);
    }

    public void Playtest()
    {
        GameController controller = GameController.Instance;
        if (controller == null)
        {
            Debug.LogWarning("Main Menu needs a persistent GameController to start a playtest.");
            return;
        }

        controller.LoadSceneByPath(playtestScenePath, null, false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
