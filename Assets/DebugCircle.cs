using UnityEngine;

public class DebugCircle : MonoBehaviour
{
    public float radius = 2f;
    public int segments = 32;
    private LineRenderer lineRenderer;

    void Start()
    {
        // Create LineRenderer for visual
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Use default sprite material for visibility
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        DrawCircle();
    }

    void DrawCircle()
    {
        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += 360f / segments;
        }
    }

    void Update()
    {
        // Update circle if radius changes in inspector
        if (lineRenderer != null)
        {
            DrawCircle();
        }
    }

    // Visualize in Scene view too
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
