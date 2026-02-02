using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    private AudioSource sfxSource; // dont need for now, auto gen in awake

    [Header("Sound Effects")]
    public AudioClip jumpSound;
    public AudioClip liftSound;
    public AudioClip powerupSound;
    public AudioClip hitColliderSound;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (sfxSource == null) {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayOneShot(AudioClip clip) {
        if (clip != null && sfxSource != null) {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayOneShot(AudioClip clip, float volume) {
        if (clip != null && sfxSource != null) {
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayJump(float volume = 1.0f) => PlayOneShot(jumpSound, volume);
    public void PlayLift(float volume = 1.0f) => PlayOneShot(liftSound, volume);
    public void PlayPowerup(float volume = 1.0f) => PlayOneShot(powerupSound, volume);
    public void PlayHitCollider(float volume = 1.0f) => PlayOneShot(hitColliderSound, volume);
}
