using UnityEngine;

// Finds the closest IInteractable in range and triggers it when the Interact input fires.
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerInteractor : MonoBehaviour
{
    public float interactRadius = 1.5f;
    public LayerMask interactableLayerMask = ~0;

    private PlayerInputHandler input;
    private readonly Collider2D[] results = new Collider2D[8];

    private void Awake()
    {
        input = GetComponent<PlayerInputHandler>();
    }

    private void OnEnable() => input.InteractPressed += HandleInteractPressed;
    private void OnDisable() => input.InteractPressed -= HandleInteractPressed;

    private void HandleInteractPressed()
    {
        // Don't let Interact also advance/dismiss whatever dialogue is currently open.
        if (DialogueUI.IsOpen) return;

        IInteractable closest = FindClosestInteractable();
        closest?.Interact();
    }

    private IInteractable FindClosestInteractable()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, interactRadius, results, interactableLayerMask);
        IInteractable closest = null;
        float closestSqrDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = results[i];
            if (hit == null) continue;

            IInteractable interactable = hit.GetComponentInParent<IInteractable>();
            if (interactable == null) continue;

            float sqrDistance = ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closest = interactable;
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
