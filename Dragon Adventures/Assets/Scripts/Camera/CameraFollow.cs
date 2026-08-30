using System.Collections;
using UnityEngine;

// Follows a target on a fixed 2D plane: tracks target X/Y, never rotates, keeps its own Z depth.
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector2 offset = Vector2.zero;

    [Header("Smoothing")]
    public float smoothTime = 0.15f;

    [Header("Vertical Limits")]
    public bool useVerticalLimits;
    public float topLimit = 10f;
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
