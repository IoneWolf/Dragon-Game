using System;

// A single choice presented to the player during a dialogue line.
public class DialogueChoice
{
    public readonly string Text;
    public readonly Action OnChosen;

    public DialogueChoice(string text, Action onChosen)
    {
        Text = text;
        OnChosen = onChosen;
    }
}
