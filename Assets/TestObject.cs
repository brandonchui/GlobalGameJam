using UnityEngine;
using UnityEngine.InputSystem;

public class TestObject : MonoBehaviour
{
    public Color insideColor = Color.green;
    public Color outsideColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propBlock;
    private DebugCircle circle;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circle = FindFirstObjectByType<DebugCircle>();
        propBlock = new MaterialPropertyBlock();

        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateDefaultSprite();
        }

        // Apply the circle detect shader
        Shader shader = Shader.Find("Custom/CircleDetect");
        if (shader != null)
        {
            spriteRenderer.material = new Material(shader);
        }
    }

    void Update()
    {
        if (circle == null || spriteRenderer == null) return;

        // Drag with mouse for testing
        var mouse = Mouse.current;
        if (mouse != null && mouse.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
            transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }

        // Update shader with circle info
        Material mat = spriteRenderer.material;
        mat.SetColor("_InsideColor", insideColor);
        mat.SetColor("_OutsideColor", outsideColor);
        mat.SetVector("_CircleCenter", circle.transform.position);
        mat.SetFloat("_CircleRadius", circle.radius);
    }

    Sprite CreateDefaultSprite()
    {
        Texture2D tex = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        for (int i = 0; i < colors.Length; i++) colors[i] = Color.white;
        tex.SetPixels(colors);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
    }
}
