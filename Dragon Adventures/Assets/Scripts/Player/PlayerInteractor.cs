using UnityEngine;

// Finds the closest IInteractable in range and triggers it when the Interact input fires.
// Also keeps a "you can interact" prompt shown on whichever interactable is currently closest.
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerInteractor : MonoBehaviour
{
    [Tooltip("Maximum world-space distance for finding an interactable when the Interact input is pressed.")]
    public float interactRadius = 1.5f;
    [Tooltip("Physics layers searched for interactable colliders.")]
    public LayerMask interactableLayerMask = ~0;

    private PlayerInputHandler input;
    private readonly Collider2D[] results = new Collider2D[8];
    private IInteractionPrompt currentPrompt;

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void OnEnable() => input.InteractPressed += HandleInteractPressed;

    private void OnDisable()
    {
        input.InteractPressed -= HandleInteractPressed;
        SetCurrentPrompt(null);
    }

    private void Update()
    {
        // Hide the prompt while a conversation is open so it doesn't float over the dialogue box.
        Collider2D closest = DialogueUI.IsOpen ? null : FindClosestInteractableCollider();
        SetCurrentPrompt(closest != null ? closest.GetComponentInParent<IInteractionPrompt>() : null);
    }

    private void HandleInteractPressed()
    {
        // Don't let Interact also advance/dismiss whatever dialogue is currently open.
        if (DialogueUI.IsOpen) return;

        Collider2D closest = FindClosestInteractableCollider();
        closest?.GetComponentInParent<IInteractable>()?.Interact();
    }

    private void SetCurrentPrompt(IInteractionPrompt prompt)
    {
        if (prompt == currentPrompt) return;
        currentPrompt?.SetPromptVisible(false);
        currentPrompt = prompt;
        currentPrompt?.SetPromptVisible(true);
    }

    private Collider2D FindClosestInteractableCollider()
    {
        ContactFilter2D interactableFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = interactableLayerMask,
            useTriggers = Physics2D.queriesHitTriggers
        };
        int count = Physics2D.OverlapCircle(transform.position, interactRadius, interactableFilter, results);
        Collider2D closest = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = results[i];
            if (hit == null) continue;
            if (hit.GetComponentInParent<IInteractable>() == null) continue;

            float sqrDistance = ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closest = hit;
            }
        }
        return closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
