using UnityEngine;

// Walks back and forth; flips 180 degrees and reverses direction when it hits something ahead.
// Uses a Dynamic Rigidbody2D so gravity applies the same way it does for the player.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyPatrol : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float wallCheckDistance = 0.2f;
    public LayerMask obstacleLayerMask = ~0;

    [Header("Anti-Snag")]
    [Tooltip("If trying to move but not actually advancing this long, nudge upward to pop free of a tile seam.")]
    public float snagTimeThreshold = 0.1f;
    public float snagNudgeAmount = 0.05f;

    // 1 = moving right (+X), -1 = moving left (-X).
    private int direction = 1;
    private Collider2D ownCollider;
    private Rigidbody2D rb;
    private float snagTimer;
    private float lastPositionX;

    private void Awake()
    {
        ownCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        // Otherwise a stationary/approaching Player gets treated as a wall, causing an
        // unintended flip that looks like bouncing off them.
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer != -1)
            obstacleLayerMask &= ~(1 << playerLayer);
    }

    private void FixedUpdate()
    {
        if (IsWallAhead())
            Flip();

        // Only drive horizontal velocity - gravity (via rb.gravityScale) keeps handling the Y axis.
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction * moveSpeed;
        rb.linearVelocity = velocity;

        HandleSnag();
    }

    // Same seam-snag safety net as PlayerController: if pushing a direction but not actually
    // moving, nudge upward slightly to pop free.
    private void HandleSnag()
    {
        float movedThisStep = Mathf.Abs(rb.position.x - lastPositionX);

        if (movedThisStep < 0.001f)
        {
            snagTimer += Time.fixedDeltaTime;
            if (snagTimer >= snagTimeThreshold)
            {
                rb.position += Vector2.up * snagNudgeAmount;
                snagTimer = 0f;
            }
        }
        else
        {
            snagTimer = 0f;
        }

        lastPositionX = rb.position.x;
    }

    private bool IsWallAhead()
    {
        Bounds bounds = ownCollider.bounds;
        Vector2 checkPoint = GetCheckPoint(bounds);
        Vector2 checkSize = new Vector2(0.1f, bounds.size.y * 0.9f);

        // A static overlap test (not a sweep) so it reliably detects a wall even once already
        // touching it - BoxCast can miss that case depending on the "Queries Start In Colliders" setting.
        Collider2D hit = Physics2D.OverlapBox(checkPoint, checkSize, 0f, obstacleLayerMask);
        return hit != null && hit != ownCollider;
    }

    private Vector2 GetCheckPoint(Bounds bounds)
    {
        float checkDistance = bounds.extents.x + wallCheckDistance;
        return (Vector2)bounds.center + Vector2.right * direction * checkDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D col = ownCollider != null ? ownCollider : GetComponent<Collider2D>();
        if (col == null) return;

        Bounds bounds = col.bounds;
        Vector2 checkPoint = GetCheckPoint(bounds);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(bounds.center, checkPoint);
        Gizmos.DrawWireCube(checkPoint, new Vector3(0.1f, bounds.size.y * 0.9f, 0.1f));
    }

    private void Flip()
    {
        direction *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
}
