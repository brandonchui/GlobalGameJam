using UnityEngine;

public class TestObject : MonoBehaviour
{
    public Color insideColor = Color.green;
    public Color overlapColor = Color.yellow;

    private SpriteRenderer spriteRenderer;
    private DebugCircle[] circles;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Apply the circle detect shader
        Shader shader = Shader.Find("Custom/CircleDetect");
        if (shader != null)
        {
            spriteRenderer.material = new Material(shader);
        }
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        // Find all circles each frame (in case they're added/removed)
        circles = FindObjectsByType<DebugCircle>(FindObjectsSortMode.None);

        Material mat = spriteRenderer.material;
        mat.SetColor("_InsideColor", insideColor);
        mat.SetColor("_OverlapColor", overlapColor);
        mat.SetInt("_CircleCount", circles.Length);

        // Pass circle data as arrays
        Vector4[] centers = new Vector4[10];
        float[] radii = new float[10];

        for (int i = 0; i < circles.Length && i < 10; i++)
        {
            centers[i] = circles[i].transform.position;
            radii[i] = circles[i].radius;
        }

        mat.SetVectorArray("_CircleCenters", centers);
        mat.SetFloatArray("_CircleRadii", radii);
    }
}
