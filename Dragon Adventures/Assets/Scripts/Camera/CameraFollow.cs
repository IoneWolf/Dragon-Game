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

    [Header("Corner Markers")]
    [Tooltip("Clamp the camera to Camera Bounds Corner markers placed in this scene. Requires one Bottom Left and one Top Right marker.")]
    public bool useCornerMarkers = true;

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
    private Camera sceneCamera;
    private CameraBoundsCorner bottomLeftMarker;
    private CameraBoundsCorner topRightMarker;

    private void Awake()
    {
        fixedZ = transform.position.z;
        sceneCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        FindCornerMarkers();
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
        float desiredX = target.position.x + offset.x;
        float desiredY = target.position.y + offset.y;

        if (useCornerMarkers && TryGetMarkerBounds(out Bounds markerBounds))
        {
            float halfHeight = sceneCamera != null && sceneCamera.orthographic ? sceneCamera.orthographicSize : 0f;
            float halfWidth = halfHeight * (sceneCamera != null ? sceneCamera.aspect : 1f);
            desiredX = ClampCameraCenter(desiredX, markerBounds.min.x, markerBounds.max.x, halfWidth);
            desiredY = ClampCameraCenter(desiredY, markerBounds.min.y, markerBounds.max.y, halfHeight);
        }
        else if (useVerticalLimits)
        {
            desiredY = Mathf.Clamp(desiredY, bottomLimit, topLimit);
        }

        return new Vector3(desiredX, desiredY, fixedZ);
    }

    private bool TryGetMarkerBounds(out Bounds markerBounds)
    {
        if (bottomLeftMarker == null || topRightMarker == null)
            FindCornerMarkers();

        if (bottomLeftMarker == null || topRightMarker == null)
        {
            markerBounds = default;
            return false;
        }

        Vector3 minimum = Vector3.Min(bottomLeftMarker.transform.position, topRightMarker.transform.position);
        Vector3 maximum = Vector3.Max(bottomLeftMarker.transform.position, topRightMarker.transform.position);
        markerBounds = new Bounds((minimum + maximum) * 0.5f, maximum - minimum);
        return true;
    }

    private void FindCornerMarkers()
    {
        bottomLeftMarker = null;
        topRightMarker = null;

        foreach (CameraBoundsCorner marker in FindObjectsByType<CameraBoundsCorner>(FindObjectsSortMode.None))
        {
            if (marker.gameObject.scene != gameObject.scene)
                continue;

            if (marker.cornerType == CameraBoundsCornerType.BottomLeft)
                bottomLeftMarker = marker;
            else if (marker.cornerType == CameraBoundsCornerType.TopRight)
                topRightMarker = marker;
        }
    }

    private static float ClampCameraCenter(float desired, float minimum, float maximum, float halfViewportSize)
    {
        float minimumCenter = minimum + halfViewportSize;
        float maximumCenter = maximum - halfViewportSize;
        return minimumCenter > maximumCenter
            ? (minimum + maximum) * 0.5f
            : Mathf.Clamp(desired, minimumCenter, maximumCenter);
    }
}
