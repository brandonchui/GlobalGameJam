using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Player")) {

            var player = collider.gameObject.GetComponent<CharacterController2D>();
            if (player != null) {
                player.FreezeControls(1.0f);
                Vector3 dir = collider.gameObject.transform.position - transform.position;
                dir = dir.normalized * 10.0f;
                Debug.Log(dir);
                //dir.y = Mathf.Abs(dir.y);
                player.rigid.AddForce(dir, ForceMode2D.Impulse);
            }

            Destroy(gameObject);

        }
    }
}
