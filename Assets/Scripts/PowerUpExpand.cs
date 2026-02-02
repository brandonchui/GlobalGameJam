using UnityEngine;

public class PowerUpExpand : MonoBehaviour {
    [Header("Power Up Settings")]
    public float expandAmount = 2f;
    public float duration = 3f;

    SpriteRenderer sr;
    float pulseSpeed;

    void Start() {
        sr = GetComponentInChildren<SpriteRenderer>();
        pulseSpeed = Random.Range(5.0f, 7.0f);
    }

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
            AudioManager.Instance.PlayPowerup();
            Destroy(gameObject);
        }
    }
    void Update() {
        sr.transform.localScale = Vector3.one + Vector3.one * Mathf.Sin(Time.time * pulseSpeed) * 0.2f;
    }
}
