using UnityEngine;

// A talking rock: interacting asks the player to pick it up, and reacts if they actually do.
// Placeholder half-circle sprite (see RockVisual) - swap in real art whenever it's ready.
[RequireComponent(typeof(RockVisual))]
[RequireComponent(typeof(InteractionPromptIcon))]
public class RockInteractable : MonoBehaviour, IInteractable
{
    private RockVisual visual;

    private void Awake()
    {
        visual = GetComponent<RockVisual>();
        EnsureTriggerCollider();
    }

    // So the player doesn't need to manually add/configure a collider in the Editor.
    private void EnsureTriggerCollider()
    {
        Collider2D existing = GetComponent<Collider2D>();
        if (existing != null)
        {
            existing.isTrigger = true;
            return;
        }

        CircleCollider2D circle = gameObject.AddComponent<CircleCollider2D>();
        circle.isTrigger = true;
        circle.radius = 0.5f;
    }

    public void Interact()
    {
        DialogueUI.Show(visual.Sprite, "Pick me up", new[]
        {
            new DialogueChoice("Pick it up?", HandlePickUp),
            new DialogueChoice("Leave it?", DialogueUI.Hide)
        });
    }

    private void HandlePickUp()
    {
        DialogueUI.Show(visual.Sprite, "Woaw did you really pick that rock up because it told you to?", new[]
        {
            new DialogueChoice("...", HandlePickUpAcknowledged)
        });
    }

    private void HandlePickUpAcknowledged()
    {
        DialogueUI.Hide();
        gameObject.SetActive(false);
    }
}
