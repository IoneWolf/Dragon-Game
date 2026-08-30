using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Reads input from a PlayerInput component (Behavior: Send Messages) and exposes it to PlayerController.
[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool SprintHeld { get; private set; }
    public bool CrouchHeld { get; private set; }

    // Timestamp of the last jump press, used by PlayerController for jump buffering.
    public float LastJumpPressedTime { get; private set; } = -10f;

    // Fired on Interact press, consumed by PlayerInteractor.
    public event Action InteractPressed;

    // Matching action names in InputSystem_Actions.inputactions: Move, Jump, Sprint, Interact.
    public void OnMove(InputValue value) => MoveInput = value.Get<Vector2>();

    public void OnJump(InputValue value)
    {
        if (value.isPressed) LastJumpPressedTime = Time.time;
    }

    public void OnSprint(InputValue value) => SprintHeld = value.isPressed;

    public void ReleaseSprint()
    {
        SprintHeld = false;
    }

    public void OnCrouch(InputValue value) => CrouchHeld = value.isPressed;

    public void ReleaseCrouch()
    {
        CrouchHeld = false;
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed) InteractPressed?.Invoke();
    }

    // Called once a buffered jump has been used so it can't trigger a second jump.
    public void ConsumeJump()
    {
        LastJumpPressedTime = -10f;
    }
}
