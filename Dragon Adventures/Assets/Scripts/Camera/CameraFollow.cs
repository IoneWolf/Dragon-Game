using UnityEngine;

// Follows a target on a fixed 2D plane: tracks target X/Y, never rotates, keeps its own Z depth.
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector2 offset = Vector2.zero;

    [Header("Smoothing")]
    public float smoothTime = 0.15f;

    private Vector3 velocity;
    private float fixedZ;

    private void Awake()
    {
        fixedZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, fixedZ);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}
