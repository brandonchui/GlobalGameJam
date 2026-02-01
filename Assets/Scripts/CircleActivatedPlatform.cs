using UnityEngine;

public class CircleActivatedPlatform : MonoBehaviour {
    private Collider2D col;

    public enum CircleMode {
        AnyCircle,
        GreenCircleOnly,
        RedCircleOnly,
        BothCirclesOverlap
    }

    [Header("Settings")]
    public CircleMode mode = CircleMode.AnyCircle;

    private void Awake() {
        col = GetComponent<Collider2D>();
    }

    private void Update() {
        if (CircleManager.instance == null || col == null) return;

        col.enabled = CheckInsideCircle();
    }

    private bool CheckInsideCircle() {
        var circles = CircleManager.instance.circles;
        if (circles == null || circles.Count == 0) return false;

        Vector2 myPos = transform.position;

        bool inGreen = false;
        bool inRed = false;

        if (circles.Count > 0 && circles[0] != null) {
            float dist = Vector2.Distance(myPos, circles[0].transform.position);
            inGreen = dist <= circles[0].radius;
        }

        if (circles.Count > 1 && circles[1] != null) {
            float dist = Vector2.Distance(myPos, circles[1].transform.position);
            inRed = dist <= circles[1].radius;
        }

        switch (mode) {
            case CircleMode.AnyCircle:
                return inGreen || inRed;
            case CircleMode.GreenCircleOnly:
                return inGreen;
            case CircleMode.RedCircleOnly:
                return inRed;
            case CircleMode.BothCirclesOverlap:
                return inGreen && inRed;
            default:
                return false;
        }
    }
}
