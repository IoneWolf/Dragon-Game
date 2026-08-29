// Optional companion to IInteractable: lets PlayerInteractor show/hide a "you can interact" prompt
// as the player enters/leaves range, without the prompt logic being baked into IInteractable itself.
public interface IInteractionPrompt
{
    void SetPromptVisible(bool visible);
}
