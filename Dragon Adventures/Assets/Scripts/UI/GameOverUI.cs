using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Shows a "GAME OVER" overlay with a Restart button when the player's HP reaches 0.
public class GameOverUI : MonoBehaviour
{
    [Tooltip("Player health component that triggers this overlay when its HP reaches zero.")]
    public PlayerHealth target;
    [Tooltip("Color and transparency of the full-screen game-over overlay.")]
    public Color overlayColor = new Color(0f, 0f, 0f, 0.75f);

    private GameObject panel;

    private void Awake()
    {
        BuildUI();
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        if (target != null) target.OnPlayerDefeated += HandleDefeated;
    }

    private void OnDisable()
    {
        if (target != null) target.OnPlayerDefeated -= HandleDefeated;
    }

    public void SetTarget(PlayerHealth newTarget)
    {
        if (target == newTarget) return;

        if (target != null)
            target.OnPlayerDefeated -= HandleDefeated;

        target = newTarget;

        if (isActiveAndEnabled && target != null)
            target.OnPlayerDefeated += HandleDefeated;
    }

    private void BuildUI()
    {
        EnsureEventSystem();

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        panel = new GameObject("GameOverPanel", typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = overlayColor;

        BuildTitleText();
        BuildRestartButton();
    }

    // Without an EventSystem in the scene, UI clicks are silently ignored.
    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    private void BuildTitleText()
    {
        GameObject titleObject = new GameObject("GameOverText", typeof(Text));
        titleObject.transform.SetParent(panel.transform, false);

        Text title = titleObject.GetComponent<Text>();
        title.text = "GAME OVER";
        title.alignment = TextAnchor.MiddleCenter;
        title.fontSize = 120;
        title.fontStyle = FontStyle.Bold;
        title.color = Color.white;
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.6f);
        titleRect.anchorMax = new Vector2(0.5f, 0.6f);
        titleRect.sizeDelta = new Vector2(900f, 160f);
        titleRect.anchoredPosition = Vector2.zero;
    }

    private void BuildRestartButton()
    {
        GameObject buttonObject = new GameObject("RestartButton", typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(panel.transform, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.35f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.35f);
        buttonRect.sizeDelta = new Vector2(500f, 140f);
        buttonRect.anchoredPosition = Vector2.zero;

        buttonObject.GetComponent<Image>().color = Color.white;
        buttonObject.GetComponent<Button>().onClick.AddListener(Restart);

        GameObject labelObject = new GameObject("Text", typeof(Text));
        labelObject.transform.SetParent(buttonObject.transform, false);

        Text label = labelObject.GetComponent<Text>();
        label.text = "Restart";
        label.alignment = TextAnchor.MiddleCenter;
        label.fontSize = 56;
        label.fontStyle = FontStyle.Bold;
        label.color = Color.black;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
    }

    private void HandleDefeated()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Restart()
    {
        Time.timeScale = 1f;

        GameController controller = GameController.Instance;
        if (controller == null || !controller.RestartCurrentLevel())
            return;

        panel.SetActive(false);
    }
}
