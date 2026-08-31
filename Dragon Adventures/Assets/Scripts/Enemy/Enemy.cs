using UnityEngine;

// Deals damage to the player on side contact, but takes damage itself if the player stomps
// down on top of it. Adds its own dedicated trigger collider for detection so the enemy's main
// collider (used by EnemyPatrol for ground/wall physics) can stay solid - a single collider
// can't be both solid and a trigger at once.
[RequireComponent(typeof(EnemySpriteVisual))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(EnemyHealth))]
public class Enemy : MonoBehaviour
{
    [Tooltip("Hit points removed from the player on a non-stomp contact.")]
    public int damage = 1;
    [Tooltip("Minimum upward velocity applied to the player after stomping this enemy.")]
    public int stompBounceForce = 6;

    [Tooltip("Trigger collider used purely for damage/stomp detection. Auto-created if left empty.")]
    public Collider2D damageTrigger;
    [Tooltip("Player's feet must be at least this fraction up the enemy's height to count as a stomp.")]
    [Range(0f, 1f)]
    public float stompHeightRatio = 0.5f;

    private EnemyHealth health;
    private Collider2D solidCollider;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
        solidCollider = GetComponent<BoxCollider2D>();
        ExcludePlayerFromSolidCollision();

        if (damageTrigger != null)
        {
            damageTrigger.isTrigger = true;
            return;
        }

        BoxCollider2D solid = solidCollider as BoxCollider2D;
        BoxCollider2D trigger = gameObject.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        if (solid != null)
        {
            // Force this back to solid regardless of leftover/legacy serialized state,
            // otherwise both colliders end up as triggers and the enemy has nothing to stand on.
            solid.isTrigger = false;
            trigger.size = solid.size;
            trigger.offset = solid.offset;
        }
        damageTrigger = trigger;
    }

    // So the Player and Enemy phase through each other physically (Mario-style) instead of
    // bouncing off, while still colliding normally with the ground/walls. Only requires a
    // "Player" Layer to exist and be assigned to the Player GameObject - no per-collider
    // Inspector configuration needed.
    private void ExcludePlayerFromSolidCollision()
    {
        if (solidCollider == null) return;

        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer == -1)
        {
            Debug.LogWarning("[Enemy] No 'Player' Layer found (Project Settings > Tags and Layers). " +
                              "Add one and assign it to the Player GameObject to stop them colliding solidly.");
            return;
        }

        solidCollider.excludeLayers |= (1 << playerLayer);
    }

    private void OnTriggerEnter2D(Collider2D other) => HandleContact(other);
    private void OnTriggerStay2D(Collider2D other) => HandleContact(other);

    private void HandleContact(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
        if (playerHealth == null) return;

        Rigidbody2D playerRb = other.attachedRigidbody;

        if (IsStomp(other, playerRb))
        {
            health.TakeDamage(1);
            if (playerRb != null)
            {
                Vector2 velocity = playerRb.linearVelocity;
                velocity.y = Mathf.Max(velocity.y, stompBounceForce);
                playerRb.linearVelocity = velocity;
            }
        }
        else
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // A stomp = the player's feet are in the upper portion of the enemy AND they're falling.
    private bool IsStomp(Collider2D other, Rigidbody2D playerRb)
    {
        Bounds enemyBounds = (solidCollider != null ? solidCollider : damageTrigger).bounds;
        float stompLine = enemyBounds.min.y + enemyBounds.size.y * stompHeightRatio;
        bool feetAboveStompLine = other.bounds.min.y >= stompLine;
        bool falling = playerRb == null || playerRb.linearVelocity.y <= 0.01f;
        return feetAboveStompLine && falling;
    }
}
