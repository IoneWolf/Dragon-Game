using NUnit.Framework;
using UnityEngine;

// Unit tests for PlayerInputHandler's jump-timestamp bookkeeping.
public class PlayerInputHandlerTests
{
    private GameObject go;
    private PlayerInputHandler handler;

    [SetUp]
    public void SetUp()
    {
        go = new GameObject("TestPlayer");
        go.AddComponent<CharacterController>();
        go.AddComponent<PlayerController>();
        handler = go.AddComponent<PlayerInputHandler>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(go);
    }

    [Test]
    public void ConsumeJump_ResetsLastJumpPressedTime()
    {
        handler.ConsumeJump();

        Assert.Less(handler.LastJumpPressedTime, Time.time - 1f,
            $"[{nameof(PlayerInputHandlerTests)}] Scripts/Player/PlayerInputHandler.cs: ConsumeJump did not reset LastJumpPressedTime to a stale value.");
    }
}
