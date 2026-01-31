using UnityEngine;

public class LevelGen : MonoBehaviour {
    public GameObject greenPrefab;
    public GameObject redPrefab;
    public GameObject normalPrefab;
    public GameObject ground;
    public GameObject enemyPrefab;

    [Range(0f, 1f)]
    public float enemySpawnChance = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        float height = 50;
        float width = 10;
        float scale = 4.5f;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; ++x) {
                var prefab = Random.value < 0.2f ? normalPrefab : Random.value < 0.5f ? greenPrefab : redPrefab;
                GameObject go = Instantiate(prefab, transform);
                go.transform.position = new Vector3(x - width / 2.0f + Random.value, y + Random.value, 0) * scale;
                bool widerOrTall = Random.value < 0.55f;
                Vector2 size = new Vector2(widerOrTall ? Random.Range(2.0f, scale) : 1, !widerOrTall ? Random.Range(2.0f, scale) : 1);
                SetSize(go, size);

                // Spawn enemy on platform
                if (enemyPrefab != null && Random.value < enemySpawnChance) {
                    Vector3 enemyPos = go.transform.position + Vector3.up * (size.y / 2f + 0.5f);
                    Instantiate(enemyPrefab, enemyPos, Quaternion.identity, transform);
                }
            }
        }

        // make side walls
        GameObject leftWall = Instantiate(normalPrefab, transform);
        leftWall.transform.position = new Vector3(-width / 2.0f, height / 2.0f, 0) * scale;
        SetSize(leftWall, new Vector2(1, height*scale));

        GameObject rightWall = Instantiate(normalPrefab, transform);
        rightWall.transform.position = new Vector3(width / 2.0f, height / 2.0f, 0) * scale;
        SetSize(rightWall, new Vector2(1, height*scale));

        SetSize(ground, new Vector2(width * scale + 1, 1));
    }

    void SetSize(GameObject go, Vector2 size) {
        var sprite = go.GetComponent<SpriteRenderer>();
        var box = go.GetComponent<BoxCollider2D>();
        sprite.size = size;
        box.size = size;
    }

}
