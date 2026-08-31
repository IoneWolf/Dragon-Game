using System.Collections;
using UnityEngine;

// Follows a target on a fixed 2D plane: tracks target X/Y, never rotates, keeps its own Z depth.
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Transform for the camera to follow. Usually the Player transform.")]
    public Transform target;
    [Tooltip("World-space X/Y distance from the target used to frame the camera.")]
    public Vector2 offset = Vector2.zero;

    [Header("Smoothing")]
    [Tooltip("Seconds the camera takes to catch up. Use 0 for an instant snap.")]
    public float smoothTime = 0.15f;

    [Header("Vertical Limits")]
    [Tooltip("Clamp the camera's vertical position between Bottom Limit and Top Limit.")]
    public bool useVerticalLimits;
    [Tooltip("Highest allowed world Y position for the camera when vertical limits are enabled.")]
    public float topLimit = 10f;
    [Tooltip("Lowest allowed world Y position for the camera when vertical limits are enabled.")]
    public float bottomLimit = -10f;

    private Vector3 velocity;
    private float fixedZ;
    private Coroutine smoothingRestoreRoutine;

    private void Awake()
    {
        fixedZ = transform.position.z;
    }

    private void Start()
    {
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = GetTargetPosition();
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }

    public void SnapToTarget()
    {
        if (target == null) return;

        transform.position = GetTargetPosition();
        velocity = Vector3.zero;
    }

    public void DisableSmoothingFor(float duration)
    {
        if (smoothingRestoreRoutine != null)
            StopCoroutine(smoothingRestoreRoutine);

        smoothingRestoreRoutine = StartCoroutine(RestoreSmoothingAfterDelay(duration, smoothTime));
    }

    private IEnumerator RestoreSmoothingAfterDelay(float duration, float previousSmoothTime)
    {
        smoothTime = 0f;
        yield return new WaitForSecondsRealtime(duration);
        smoothTime = previousSmoothTime;
        smoothingRestoreRoutine = null;
    }

    private Vector3 GetTargetPosition()
    {
        float desiredY = target.position.y + offset.y;
        if (useVerticalLimits)
            desiredY = Mathf.Clamp(desiredY, bottomLimit, topLimit);

        return new Vector3(target.position.x + offset.x, desiredY, fixedZ);
    }
}
