using UnityEngine;
using UnityEngine.UI;

// Builds a simple screen-space HP bar and colors it green/yellow/red based on remaining HP percentage.
public class HealthBarUI : MonoBehaviour
{
    [Tooltip("Player health component whose HP this bar displays.")]
    public PlayerHealth target;
    [Tooltip("Width and height of the health bar in screen pixels.")]
    public Vector2 barSize = new Vector2(200f, 24f);
    [Tooltip("Pixel offset from the top-left corner of the screen.")]
    public Vector2 screenOffset = new Vector2(20f, -20f);

    private Image fillImage;
    private RectTransform fillRect;

    private void Awake()
    {
        BuildUI();
    }

    private void OnEnable()
    {
        if (target != null) target.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        if (target != null) target.OnHealthChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        if (target != null) HandleHealthChanged(target.CurrentHP, target.maxHP);
    }

    public void SetTarget(PlayerHealth newTarget)
    {
        if (target == newTarget) return;

        if (target != null)
            target.OnHealthChanged -= HandleHealthChanged;

        target = newTarget;

        if (isActiveAndEnabled && target != null)
        {
            target.OnHealthChanged += HandleHealthChanged;
            HandleHealthChanged(target.CurrentHP, target.maxHP);
        }
    }

    private void BuildUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        GameObject background = new GameObject("HealthBarBackground", typeof(Image));
        background.transform.SetParent(canvas.transform, false);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        backgroundRect.anchorMin = new Vector2(0f, 1f);
        backgroundRect.anchorMax = new Vector2(0f, 1f);
        backgroundRect.pivot = new Vector2(0f, 1f);
        backgroundRect.anchoredPosition = screenOffset;
        backgroundRect.sizeDelta = barSize;
        background.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);

        GameObject fill = new GameObject("HealthBarFill", typeof(Image));
        fill.transform.SetParent(background.transform, false);
        fillRect = fill.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        fillImage = fill.GetComponent<Image>();
        fillImage.color = Color.green;
    }

    private void HandleHealthChanged(int current, int max)
    {
        float fraction = max > 0 ? (float)current / max : 0f;
        fillRect.anchorMax = new Vector2(Mathf.Clamp01(fraction), 1f);
        fillImage.color = GetColorForFraction(fraction);
    }

    // Pure function extracted for unit testing.
    public static Color GetColorForFraction(float fraction)
    {
        if (fraction > 2f / 3f) return Color.green;
        if (fraction > 1f / 3f) return Color.yellow;
        return Color.red;
    }
}
