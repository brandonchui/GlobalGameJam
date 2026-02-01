using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCircle : MonoBehaviour
{
    public float radius = 2f;
    public int segments = 32;
    public Color color = Color.green;
    private LineRenderer lineRenderer;

    // Power up state
    private float baseRadius;
    private float powerUpTimer = 0f;
    private float powerUpDuration = 0f;
    private float expandedRadius = 0f;

    void Start()
    {
        baseRadius = radius;

        // Create LineRenderer for visual
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Use default sprite material for visibility
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

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
        // Handle power up shrinking
        if (powerUpTimer > 0f)
        {
            powerUpTimer -= Time.deltaTime;
            float t = powerUpTimer / powerUpDuration;
            radius = Mathf.Lerp(baseRadius, expandedRadius, t);
        }

        // Update circle if radius changes in inspector
        if (lineRenderer != null)
        {
            DrawCircle();
        }
    }

    public void ApplyExpandPowerUp(float expandAmount, float duration)
    {
        expandedRadius = baseRadius + expandAmount;
        radius = expandedRadius;
        powerUpDuration = duration;
        powerUpTimer = duration;
    }

    public void SetPosition(Vector3 pos) {
        transform.position = pos;
    }

}
