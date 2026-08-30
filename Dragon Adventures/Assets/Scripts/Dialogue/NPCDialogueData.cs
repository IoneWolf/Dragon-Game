using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dragon Adventure/Dialogue/NPC Dialogue", fileName = "New NPC Dialogue")]
public class NPCDialogueData : ScriptableObject
{
    public Sprite speakerSprite;
    public string openingLine = "Hey there.";
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
    public string choiceText = "Option";
    [TextArea(2, 4)] public string responseText = "Response";
    public bool closesDialogue;
}