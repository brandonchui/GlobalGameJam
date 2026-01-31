using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour {
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // Find highest player height for co-op
            float maxHeight = 0f;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players) {
                if (player.transform.position.y > maxHeight) {
                    maxHeight = player.transform.position.y;
                }
            }

            PlayerPrefs.SetFloat("PendingScore", maxHeight);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Leaderboard");
        }
    }
}
