using UnityEngine;

public class CameraController : MonoBehaviour {
    [Header("Targets")]
    public Transform[] players;

    [Header("Camera Settings")]
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float singlePlayerZoom = 10f;
    public float zoomPadding = 3f;
    public float smoothTime = 0.2f;

    [Header("Bounds")]
    public float minY = 0f;

    private Vector3 velocity;
    private Camera cam;

    private void Awake() {
        cam = GetComponent<Camera>();
    }

    private void Start() {
        // Auto-find players if not assigned
        if (players == null || players.Length == 0) {
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            players = new Transform[playerObjects.Length];
            for (int i = 0; i < playerObjects.Length; i++) {
                players[i] = playerObjects[i].transform;
            }
        }
    }

    private void LateUpdate() {
        if (players == null || players.Length == 0) return;

        // Filter out null/destroyed/inactive players
        int activeCount = 0;
        foreach (var p in players) {
            if (p != null && p.gameObject.activeInHierarchy) activeCount++;
        }
        if (activeCount == 0) return;

        // Calculate bounds of all players
        Vector3 centerPoint = GetCenterPoint();
        float requiredZoom = GetRequiredZoom();

        // Smooth camera movement
        Vector3 targetPos = new Vector3(centerPoint.x, Mathf.Max(centerPoint.y, minY), transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Smooth zoom
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, requiredZoom, Time.deltaTime * 5f);
    }

    private Vector3 GetCenterPoint() {
        // Find first active player for initial bounds
        Transform firstActive = null;
        foreach (var p in players) {
            if (p != null && p.gameObject.activeInHierarchy) {
                firstActive = p;
                break;
            }
        }
        if (firstActive == null) return transform.position;

        Bounds bounds = new Bounds(firstActive.position, Vector3.zero);
        foreach (var player in players) {
            if (player != null && player.gameObject.activeInHierarchy) {
                bounds.Encapsulate(player.position);
            }
        }
        return bounds.center;
    }

    private float GetRequiredZoom() {
        // Use wider zoom for single player
        if (!GameManager.IsCoop) return singlePlayerZoom;

        // Find first active player for initial bounds
        Transform firstActive = null;
        foreach (var p in players) {
            if (p != null && p.gameObject.activeInHierarchy) {
                firstActive = p;
                break;
            }
        }
        if (firstActive == null) return minZoom;

        Bounds bounds = new Bounds(firstActive.position, Vector3.zero);
        foreach (var player in players) {
            if (player != null && player.gameObject.activeInHierarchy) {
                bounds.Encapsulate(player.position);
            }
        }

        float sizeX = bounds.size.x;
        float sizeY = bounds.size.y;

        // Account for aspect ratio
        float aspectRatio = (float)Screen.width / Screen.height;
        float requiredZoomX = (sizeX / 2f / aspectRatio) + zoomPadding;
        float requiredZoomY = (sizeY / 2f) + zoomPadding;

        float requiredZoom = Mathf.Max(requiredZoomX, requiredZoomY);
        return Mathf.Clamp(requiredZoom, minZoom, maxZoom);
    }
}
