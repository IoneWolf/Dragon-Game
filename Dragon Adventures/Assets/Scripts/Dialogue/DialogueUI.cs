using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Runtime-built dialogue box: a speaker sprite, a line of text, and optional choice buttons.
// Auto-creates itself on first use, so any IInteractable can just call DialogueUI.Show(...).
public class DialogueUI : MonoBehaviour
{
    private static DialogueUI instance;

    public static bool IsOpen => instance != null && instance.panel.activeSelf;

    public static void Show(Sprite speakerSprite, string text, IReadOnlyList<DialogueChoice> choices = null)
    {
        EnsureInstance();
        instance.DisplayLine(speakerSprite, text, choices);
    }

    public static void Hide()
    {
        if (instance != null) instance.panel.SetActive(false);
    }

    private static void EnsureInstance()
    {
        if (instance != null) return;
        instance = new GameObject("DialogueUI").AddComponent<DialogueUI>();
    }

    private GameObject panel;
    private Image speakerImage;
    private Text bodyText;
    private Transform choiceContainer;
    private readonly List<GameObject> choiceButtons = new List<GameObject>();

    private void Awake()
    {
        BuildUI();
        panel.SetActive(false);
    }

    private void DisplayLine(Sprite speakerSprite, string text, IReadOnlyList<DialogueChoice> choices)
    {
        panel.SetActive(true);
        speakerImage.sprite = speakerSprite;
        speakerImage.enabled = speakerSprite != null;
        bodyText.text = text;

        foreach (GameObject button in choiceButtons) Destroy(button);
        choiceButtons.Clear();

        if (choices == null) return;
        foreach (DialogueChoice choice in choices)
            choiceButtons.Add(BuildChoiceButton(choice));
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

        panel = new GameObject("DialoguePanel", typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 0f);
        panelRect.anchorMax = new Vector2(1f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(0f, 300f);
        panel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.85f);

        BuildSpeakerImage();
        BuildBodyText();
        BuildChoiceContainer();
    }

    // Without an EventSystem in the scene, UI clicks are silently ignored.
    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    private void BuildSpeakerImage()
    {
        GameObject imageObject = new GameObject("SpeakerImage", typeof(Image));
        imageObject.transform.SetParent(panel.transform, false);
        speakerImage = imageObject.GetComponent<Image>();
        speakerImage.preserveAspect = true;

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = new Vector2(24f, 0f);
        rect.sizeDelta = new Vector2(160f, 160f);
    }

    private void BuildBodyText()
    {
        GameObject textObject = new GameObject("BodyText", typeof(Text));
        textObject.transform.SetParent(panel.transform, false);

        bodyText = textObject.GetComponent<Text>();
        bodyText.alignment = TextAnchor.UpperLeft;
        bodyText.fontSize = 44;
        bodyText.color = Color.white;
        bodyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(200f, -20f);
        rect.sizeDelta = new Vector2(-224f, 100f);
    }

    private void BuildChoiceContainer()
    {
        GameObject containerObject = new GameObject("Choices", typeof(VerticalLayoutGroup));
        containerObject.transform.SetParent(panel.transform, false);
        choiceContainer = containerObject.transform;

        VerticalLayoutGroup layout = containerObject.GetComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.spacing = 8f;

        RectTransform rect = containerObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = new Vector2(200f, 16f);
        rect.sizeDelta = new Vector2(-224f, -100f);
    }

    private GameObject BuildChoiceButton(DialogueChoice choice)
    {
        GameObject buttonObject = new GameObject("Choice_" + choice.Text, typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(choiceContainer, false);

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredWidth = 320f;
        layoutElement.preferredHeight = 48f;

        buttonObject.GetComponent<Image>().color = Color.white;
        buttonObject.GetComponent<Button>().onClick.AddListener(() => choice.OnChosen?.Invoke());

        GameObject labelObject = new GameObject("Text", typeof(Text));
        labelObject.transform.SetParent(buttonObject.transform, false);

        Text label = labelObject.GetComponent<Text>();
        label.text = choice.Text;
        label.alignment = TextAnchor.MiddleCenter;
        label.fontSize = 34;
        label.color = Color.black;
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return buttonObject;
    }
}
