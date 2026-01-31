using UnityEngine;
using UnityEngine.SceneManagement;

public class Lava : MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

    }
    float riseSpeed = 0.1f;

    // Update is called once per frame
    void Update() {
        transform.Translate(new Vector3(0, riseSpeed * Time.deltaTime, 0));
        riseSpeed += Time.deltaTime * 0.0025f;
        Debug.Log(riseSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag == "Player") {
            PlayerPrefs.SetFloat("PendingScore", CircleManager.instance.maxHeightBaby);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Leaderboard");
        }
    }
}
