using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dragon Adventure/Dialogue/NPC Dialogue", fileName = "New NPC Dialogue")]
public class NPCDialogueData : ScriptableObject
{
    [Tooltip("Optional portrait shown beside the dialogue. Leave empty to use the NPC's generated sprite.")]
    public Sprite speakerSprite;
    [Tooltip("First line shown when the player starts a conversation with this NPC.")]
    public string openingLine = "Hey there.";
    [Tooltip("Choices shown after the opening line. Leave empty for a single Goodbye option.")]
    public NPCDialogueOption[] options =
    {
        new NPCDialogueOption
        {
            choiceText = "Who are you?",
            responseText = "I'm just a friendly square for now."
        },
        new NPCDialogueOption
        {
            choiceText = "Any advice?",
            responseText = "Try not to stick to walls. It is bad for morale."
        },
        new NPCDialogueOption
        {
            choiceText = "Goodbye.",
            closesDialogue = true
        }
    };
}

[Serializable]
public class NPCDialogueOption
{
    [Tooltip("Text displayed on this dialogue choice button.")]
    public string choiceText = "Option";
    [Tooltip("Text displayed after this choice is selected.")]
    [TextArea(2, 4)] public string responseText = "Response";
    [Tooltip("Close the dialogue immediately when this option is selected instead of showing its response.")]
    public bool closesDialogue;
}