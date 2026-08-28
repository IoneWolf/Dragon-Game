using NUnit.Framework;
using UnityEngine;

// Unit tests for PlayerController's jump math (pure static helpers, no physics needed).
public class PlayerControllerTests
{
    [Test]
    public void CalculateJumpVelocity_MatchesPhysicsFormula()
    {
        float jumpHeight = 2f;
        float gravity = -9.81f;
        float expected = Mathf.Sqrt(jumpHeight * -2f * gravity);

        float actual = PlayerController.CalculateJumpVelocity(jumpHeight, gravity);

        Assert.AreEqual(expected, actual, 0.0001f,
            $"[{nameof(PlayerControllerTests)}] Scripts/Player/PlayerController.cs: CalculateJumpVelocity formula result is wrong.");
    }

    [Test]
    public void CanJump_WithinCoyoteAndBufferWindows_ReturnsTrue()
    {
        bool result = PlayerController.CanJump(now: 10f, lastGroundedTime: 9.9f, lastJumpPressedTime: 9.95f, coyoteTime: 0.15f, jumpBufferTime: 0.15f);

        Assert.IsTrue(result,
            $"[{nameof(PlayerControllerTests)}] Scripts/Player/PlayerController.cs: CanJump should be true within coyote/buffer windows.");
    }

    [Test]
    public void CanJump_GroundedTooLongAgo_ReturnsFalse()
    {
        bool result = PlayerController.CanJump(now: 10f, lastGroundedTime: 5f, lastJumpPressedTime: 9.95f, coyoteTime: 0.15f, jumpBufferTime: 0.15f);

        Assert.IsFalse(result,
            $"[{nameof(PlayerControllerTests)}] Scripts/Player/PlayerController.cs: CanJump should be false once coyote time expires.");
    }

    [Test]
    public void CanJump_PressedTooLongAgo_ReturnsFalse()
    {
        bool result = PlayerController.CanJump(now: 10f, lastGroundedTime: 9.99f, lastJumpPressedTime: 5f, coyoteTime: 0.15f, jumpBufferTime: 0.15f);

        Assert.IsFalse(result,
            $"[{nameof(PlayerControllerTests)}] Scripts/Player/PlayerController.cs: CanJump should be false once the jump buffer expires.");
    }
}
