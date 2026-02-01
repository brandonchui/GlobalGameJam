using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    private ParticleSystem ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Play();
        Destroy(gameObject, ps.main.duration);
    }
}
