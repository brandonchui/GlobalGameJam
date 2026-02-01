using UnityEngine;

public class Enemy : MonoBehaviour {
    [Header("Knockback Settings")]
    public float knockbackForce = 25f;
    public float upwardBias = 0.5f; // Extra upward push (0 = none, 1 = strong)
    public float freezeDuration = 0.4f;

    public ParticleSystem boom;
    float bounceSpeed;
    float wobbleSpeed;

    SpriteRenderer sr;
    void Start() {
        sr = GetComponentInChildren<SpriteRenderer>();
        bounceSpeed = Random.Range(5.0f, 7.0f);
        wobbleSpeed = Random.Range(3.0f, 5.0f);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Player")) {
            var player = collider.gameObject.GetComponent<CharacterController2D>();
            if (player != null) {
                // Calculate direction away from bomb
                Vector2 dir = (collider.transform.position - transform.position).normalized;

                // Add upward bias for more satisfying launch
                dir.y = Mathf.Max(dir.y, upwardBias);
                dir = dir.normalized;

                // Reset velocity first for consistent knockback
                player.rigid.linearVelocity = Vector2.zero;

                // Apply force
                player.rigid.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
                player.FreezeControls(freezeDuration);

                boom.transform.parent = null;
                boom.Play();
                Destroy(boom, 5.0f);
            }

            Destroy(gameObject);
        }
    }
    void Update() {
        sr.transform.localScale = new Vector3(1.0f, 1.0f + 0.2f * Mathf.Sin(Time.time * bounceSpeed), 0.0f);
        sr.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Sin(Time.time * wobbleSpeed) * 10f);
    }
}
