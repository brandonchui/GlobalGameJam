using UnityEngine;

public class PowerUpExpand : MonoBehaviour {
    [Header("Power Up Settings")]
    public float expandAmount = 2f;
    public float duration = 3f;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            // Apply to all circles via CircleManager for consistency
            if (CircleManager.instance != null) {
                foreach (var circle in CircleManager.instance.circles) {
                    if (circle != null) {
                        circle.ApplyExpandPowerUp(expandAmount, duration);
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
