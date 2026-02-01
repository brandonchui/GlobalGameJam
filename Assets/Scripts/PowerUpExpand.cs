using UnityEngine;

public class PowerUpExpand : MonoBehaviour {
    [Header("Power Up Settings")]
    public float expandAmount = 2f;
    public float duration = 3f;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            var controller = other.GetComponent<CharacterController2D>();
            if (controller != null) {
                // Apply to main circle
                if (controller.myCircle != null) {
                    controller.myCircle.ApplyExpandPowerUp(expandAmount, duration);
                }
                // In single player, also apply to second circle
                if (controller.singlePlayerMode && controller.secondCircle != null) {
                    controller.secondCircle.ApplyExpandPowerUp(expandAmount, duration);
                }
            }
            Destroy(gameObject);
        }
    }
}
