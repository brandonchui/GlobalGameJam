using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour {
    public GameObject greenPrefab;
    public GameObject redPrefab;
    public GameObject normalPrefab;
    public GameObject ground;
    public GameObject enemyPrefab;

    [Header("Generation Settings")]
    [Range(0f, 1f)]
    public float enemySpawnChance = 0.1f;
    public float width = 10;
    public float scale = 4.5f;
    public int batchSize = 10; // Rows to generate per batch
    public float generateAheadDistance = 50f; // How far ahead to generate

    private int generatedUpToRow = 0;
    private GameObject leftWall;
    private GameObject rightWall;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start() {
        // Generate initial batch
        GenerateBatch(batchSize);

        // Create initial walls
        leftWall = Instantiate(normalPrefab, transform);
        rightWall = Instantiate(normalPrefab, transform);
        UpdateWalls();

        SetSize(ground, new Vector2(width * scale + 1, 1));
    }

    void Update() {
        float cameraY = Camera.main.transform.position.y;
        float generatedHeight = generatedUpToRow * scale;

        // Generate more if camera is approaching the top
        if (cameraY + generateAheadDistance > generatedHeight) {
            GenerateBatch(batchSize);
            UpdateWalls();
        }

    }

    void GenerateBatch(int rows) {
        int startRow = generatedUpToRow;
        int endRow = startRow + rows;

        for (int y = startRow; y < endRow; y++) {
            for (int x = 0; x < (int)width; ++x) {
                var prefab = Random.value < 0.2f ? normalPrefab : Random.value < 0.5f ? greenPrefab : redPrefab;
                GameObject go = Instantiate(prefab, transform);
                go.transform.position = new Vector3(x - width / 2.0f + Random.value, y + Random.value, 0) * scale;
                bool widerOrTall = Random.value < 0.55f;
                Vector2 size = new Vector2(widerOrTall ? Random.Range(2.0f, scale) : 1, !widerOrTall ? Random.Range(2.0f, scale) : 1);
                SetSize(go, size);
                spawnedObjects.Add(go);

                // Spawn enemy on platform
                if (enemyPrefab != null && Random.value < enemySpawnChance) {
                    Vector3 enemyPos = go.transform.position + Vector3.up * (size.y / 2f + 0.5f);
                    var enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity, transform);
                    spawnedObjects.Add(enemy);
                }
            }
        }

        generatedUpToRow = endRow;
    }

    void UpdateWalls() {
        float wallHeight = generatedUpToRow * scale;
        float wallY = wallHeight / 2f;

        leftWall.transform.position = new Vector3(-width / 2.0f * scale, wallY, 0);
        SetSize(leftWall, new Vector2(1, wallHeight + 10));

        rightWall.transform.position = new Vector3(width / 2.0f * scale, wallY, 0);
        SetSize(rightWall, new Vector2(1, wallHeight + 10));
    }

    public void CleanupBelow(float yThreshold) {
        for (int i = spawnedObjects.Count - 1; i >= 0; i--) {
            if (spawnedObjects[i] == null) {
                spawnedObjects.RemoveAt(i);
                continue;
            }
            if (spawnedObjects[i].transform.position.y < yThreshold) {
                Destroy(spawnedObjects[i]);
                spawnedObjects.RemoveAt(i);
            }
        }
    }

    void SetSize(GameObject go, Vector2 size) {
        var sprite = go.GetComponent<SpriteRenderer>();
        var box = go.GetComponent<BoxCollider2D>();
        sprite.size = size;
        box.size = size;
    }

    // Draw threshold line in editor
    private void OnDrawGizmos() {
        float generatedHeight = generatedUpToRow * scale;
        float thresholdY = generatedHeight - generateAheadDistance;

        // Green line = generation threshold (when camera crosses this, new batch spawns)
        Gizmos.color = Color.green;
        Vector3 left = new Vector3(-width * scale, thresholdY, 0);
        Vector3 right = new Vector3(width * scale, thresholdY, 0);
        Gizmos.DrawLine(left, right);

        // Yellow line = current generated height
        Gizmos.color = Color.yellow;
        Vector3 genLeft = new Vector3(-width * scale, generatedHeight, 0);
        Vector3 genRight = new Vector3(width * scale, generatedHeight, 0);
        Gizmos.DrawLine(genLeft, genRight);

    }
}
