using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCam : MonoBehaviour {
    [Header("Targets")]
    public Transform player1;
    public Transform player2;

    [Header("Position")]
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float positionSmoothTime = 0.2f;

    [Header("Zoom")]
    public float minZoom = 5f;      // how close we can zoom in
    public float maxZoom = 12f;     // how far we can zoom out
    public float zoomLimiter = 10f; // higher = less zoom change
    public float zoomSmoothTime = 0.2f;

    [Header("Padding")]
    public float padding = 2f; // extra space around players

    [Header("Stability")]
    public float positionSnapThreshold = 0.01f;
    public float zoomSnapThreshold = 0.01f;


    private Camera cam;
    private Vector3 positionVelocity;
    private float zoomVelocity;

    private void Awake() {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    private void LateUpdate() {
        if (!player1 || !player2) return;

        MoveCamera();
        ZoomCamera();
    }

    void MoveCamera() {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 targetPosition = centerPoint + offset;

        if (Vector3.Distance(transform.position, targetPosition) < positionSnapThreshold) {
            transform.position = targetPosition;
            positionVelocity = Vector3.zero;
        }
        else {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref positionVelocity,
                positionSmoothTime
            );
        }
    }


    void ZoomCamera() {
        float distance = GetGreatestDistance() + padding;
        float targetZoom = Mathf.Lerp(minZoom, maxZoom, distance / zoomLimiter);

        if (Mathf.Abs(cam.orthographicSize - targetZoom) < zoomSnapThreshold) {
            cam.orthographicSize = targetZoom;
            zoomVelocity = 0f;
        }
        else {
            cam.orthographicSize = Mathf.SmoothDamp(
                cam.orthographicSize,
                targetZoom,
                ref zoomVelocity,
                zoomSmoothTime
            );
        }
    }

    Vector3 GetCenterPoint() {
        return (player1.position + player2.position) / 2f;
    }

    float GetGreatestDistance() {
        Bounds bounds = new Bounds(player1.position, Vector3.zero);
        bounds.Encapsulate(player2.position);

        return Mathf.Max(bounds.size.x, bounds.size.y);
    }
}
