using UnityEngine;

// Generic talking NPC. Dialogue content lives in an optional NPCDialogueData asset.
[AddComponentMenu("Dragon Adventure/NPC/NPC Interactable")]
[RequireComponent(typeof(NPCVisual))]
[RequireComponent(typeof(InteractionPromptIcon))]
public class NPCInteractable : MonoBehaviour, IInteractable
{
    public NPCDialogueData dialogueData;

    private NPCVisual visual;

    private void Awake()
    {
        visual = GetComponent<NPCVisual>();
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
        ShowOpeningLine();
    }

    private void ShowOpeningLine()
    {
        Sprite speakerSprite = GetSpeakerSprite();
        string openingLine = dialogueData != null ? dialogueData.openingLine : "Hey there.";
        NPCDialogueOption[] options = dialogueData != null ? dialogueData.options : null;

        if (options == null || options.Length == 0)
        {
            DialogueUI.Show(speakerSprite, openingLine, new[]
            {
                new DialogueChoice("Goodbye.", DialogueUI.Hide)
            });
            return;
        }

        DialogueChoice[] choices = new DialogueChoice[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            NPCDialogueOption option = options[i];
            choices[i] = new DialogueChoice(option.choiceText, () => HandleOptionChosen(option));
        }

        DialogueUI.Show(speakerSprite, openingLine, choices);
    }

    private void HandleOptionChosen(NPCDialogueOption option)
    {
        if (option.closesDialogue)
        {
            DialogueUI.Hide();
            return;
        }

        DialogueUI.Show(GetSpeakerSprite(), option.responseText, new[]
        {
            new DialogueChoice("Back", ShowOpeningLine),
            new DialogueChoice("Goodbye.", DialogueUI.Hide)
        });
    }

    private Sprite GetSpeakerSprite()
    {
        if (dialogueData != null && dialogueData.speakerSprite != null)
            return dialogueData.speakerSprite;
        return visual != null ? visual.Sprite : null;
    }
}