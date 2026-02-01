using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public Sprite[] sprites;
    private SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SwitchSprite();
    }

    public void SwitchSprite() {
        sr.sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
