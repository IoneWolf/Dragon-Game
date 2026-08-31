using UnityEngine;

// Small floating "Press E" label shown above an interactable while the player is in range.
// Reusable across any IInteractable - shown/hidden by PlayerInteractor via IInteractionPrompt.
public class InteractionPromptIcon : MonoBehaviour, IInteractionPrompt
{
    [Tooltip("Text shown while the player is close enough to interact.")]
    public string promptText = "Press E";
    [Tooltip("Local-space position of the prompt relative to this interactable.")]
    public Vector3 offset = new Vector3(0f, 1f, 0f);
    [Tooltip("Color of the floating prompt text.")]
    public Color textColor = Color.white;
    [Tooltip("Font size used by the floating prompt text.")]
    public int fontSize = 24;

    private GameObject promptObject;
    private TextMesh promptTextMesh;

    private void Awake()
    {
        BuildPrompt();
        SetPromptVisible(false);
    }

    private void BuildPrompt()
    {
        promptObject = new GameObject("InteractPrompt", typeof(TextMesh));
        promptObject.transform.SetParent(transform, false);
        promptObject.transform.localPosition = offset;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        promptTextMesh = promptObject.GetComponent<TextMesh>();
        promptTextMesh.text = promptText;
        promptTextMesh.characterSize = 0.1f;
        promptTextMesh.fontSize = fontSize;
        promptTextMesh.anchor = TextAnchor.MiddleCenter;
        promptTextMesh.alignment = TextAlignment.Center;
        promptTextMesh.color = textColor;
        promptTextMesh.font = font;

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = font.material;
        meshRenderer.sortingOrder = 10;
    }

    public void SetPromptVisible(bool visible)
    {
        if (promptObject != null) promptObject.SetActive(visible);
    }

    public void SetPromptText(string text)
    {
        promptText = text;
        if (promptTextMesh != null) promptTextMesh.text = promptText;
    }
}
