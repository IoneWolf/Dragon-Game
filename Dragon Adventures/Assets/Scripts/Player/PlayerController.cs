using UnityEngine;

// 2D-plane movement using Rigidbody2D, compatible with Tilemap/TilemapCollider2D (2D physics).
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.6f;

    [Header("Jumping")]
    public float jumpHeight = 1.5f;

    [Header("Jump Assist")]
    [Tooltip("Seconds after walking off an edge that a jump still counts.")]
    public float coyoteTime = 0.15f;
    [Tooltip("Seconds a jump press is remembered before landing, so it isn't dropped.")]
    public float jumpBufferTime = 0.15f;

    [Header("Ground Check")]
    [Tooltip("Empty child Transform positioned at the character's feet.")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayerMask = ~0;

    [Header("Anti-Snag")]
    [Tooltip("If grounded, trying to move, but not actually advancing this long, nudge upward to pop free of a tile seam.")]
    public float snagTimeThreshold = 0.1f;
    public float snagNudgeAmount = 0.05f;

    private Rigidbody2D rb;
    private Collider2D ownCollider;
    private PlayerInputHandler input;
    private PlayerSpriteVisual visual;
    private float lastGroundedTime = -10f;
    private float snagTimer;
    private float lastPositionX;
    private readonly Collider2D[] groundCheckResults = new Collider2D[8];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ownCollider = GetComponent<Collider2D>();
        input = GetComponent<PlayerInputHandler>();
        visual = GetComponentInChildren<PlayerSpriteVisual>();
    }

    private void Update()
    {
        if (DialogueUI.IsOpen) return;

        // Visual-only, so it updates every rendered frame instead of only on physics steps.
        float horizontal = input.MoveInput.x;
        if (visual != null && Mathf.Abs(horizontal) > 0.01f)
            visual.SetFacing(horizontal);
    }

    private void FixedUpdate()
    {
        // Freeze horizontal movement/jumping while a dialogue is open, but keep gravity acting normally.
        if (DialogueUI.IsOpen)
        {
            Vector2 frozenVelocity = rb.linearVelocity;
            frozenVelocity.x = 0f;
            rb.linearVelocity = frozenVelocity;
            return;
        }

        float horizontal = input.MoveInput.x;
        float speed = moveSpeed * (input.SprintHeld ? sprintMultiplier : 1f);

        if (IsGrounded())
            lastGroundedTime = Time.time;

        Vector2 velocity = rb.linearVelocity;

        if (CanJump(Time.time, lastGroundedTime, input.LastJumpPressedTime, coyoteTime, jumpBufferTime))
        {
            float gravity = Physics2D.gravity.y * rb.gravityScale;
            velocity.y = CalculateJumpVelocity(jumpHeight, gravity);
            input.ConsumeJump();
            lastGroundedTime = -10f;
        }

        velocity.x = horizontal * speed;
        rb.linearVelocity = velocity;

        HandleSnag(horizontal);
    }

    // Rarely, a Rigidbody2D can catch on a seam vertex between adjacent tile colliders even with
    // a Composite Collider2D. If grounded + pushing a direction but not actually moving, pop free.
    private void HandleSnag(float horizontal)
    {
        bool tryingToMove = Mathf.Abs(horizontal) > 0.1f;
        bool grounded = Time.time - lastGroundedTime < 0.05f;
        float movedThisStep = Mathf.Abs(rb.position.x - lastPositionX);

        if (tryingToMove && grounded && movedThisStep < 0.001f)
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

    private bool IsGrounded()
    {
        Vector2 origin = groundCheck != null ? (Vector2)groundCheck.position : ComputeFallbackFeetPosition();
        int count = Physics2D.OverlapCircleNonAlloc(origin, groundCheckRadius, groundCheckResults, groundLayerMask);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = groundCheckResults[i];
            // Ignore our own collider so we don't detect ourselves as "ground".
            if (hit != null && hit != ownCollider && hit.attachedRigidbody != rb)
                return true;
        }
        return false;
    }

    private Vector2 ComputeFallbackFeetPosition()
    {
        if (ownCollider != null)
            return new Vector2(ownCollider.bounds.center.x, ownCollider.bounds.min.y);
        return (Vector2)transform.position + Vector2.down * 0.5f;
    }

    // Pure functions extracted for unit testing (no Unity scene/physics dependencies).
    public static bool CanJump(float now, float lastGroundedTime, float lastJumpPressedTime, float coyoteTime, float jumpBufferTime)
    {
        bool withinCoyoteTime = now - lastGroundedTime <= coyoteTime;
        bool hasBufferedJump = now - lastJumpPressedTime <= jumpBufferTime;
        return withinCoyoteTime && hasBufferedJump;
    }

    public static float CalculateJumpVelocity(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
