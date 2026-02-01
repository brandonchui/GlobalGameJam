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
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayJump() => PlayOneShot(jumpSound);
    public void PlayLift() => PlayOneShot(liftSound);
    public void PlayPowerup() => PlayOneShot(powerupSound);
    public void PlayHitCollider() => PlayOneShot(hitColliderSound);
}
