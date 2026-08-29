using UnityEngine;

// Small floating "Press E" label shown above an interactable while the player is in range.
// Reusable across any IInteractable - shown/hidden by PlayerInteractor via IInteractionPrompt.
public class InteractionPromptIcon : MonoBehaviour, IInteractionPrompt
{
    public string promptText = "Press E";
    public Vector3 offset = new Vector3(0f, 1f, 0f);
    public Color textColor = Color.white;
    public int fontSize = 24;

    private GameObject promptObject;

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

        TextMesh textMesh = promptObject.GetComponent<TextMesh>();
        textMesh.text = promptText;
        textMesh.characterSize = 0.1f;
        textMesh.fontSize = fontSize;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = textColor;
        textMesh.font = font;

        MeshRenderer meshRenderer = promptObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = font.material;
        meshRenderer.sortingOrder = 10;
    }

    public void SetPromptVisible(bool visible)
    {
        if (promptObject != null) promptObject.SetActive(visible);
    }
}
