using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Random Corbeau Sounds")]
    public AudioClip[] randomCorbeau;

    [Header("Random Shoot Sounds")]
    public AudioClip[] randomShoot;

    [Header("Single Sounds")]
    public AudioClip explosion;
    public AudioClip smoke;
    public AudioClip click;
    public AudioClip dead;
    public AudioClip releaseObject;
    public AudioClip tackObject;
    public AudioClip kick;
    public AudioClip destroyBarricade;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // ------- Random Sounds -------
    public void PlayRandomCorbeau()
    {
        PlayRandomFromArray(randomCorbeau);
    }

    public void PlayRandomShoot()
    {
        PlayRandomFromArray(randomShoot);
    }

    // ------- Single Sounds -------
    public void PlayExplosion() => PlayClip(explosion);
    public void PlaySmoke() => PlayClip(smoke);
    public void PlayClick() => PlayClip(click);
    public void PlayDead() => PlayClip(dead);
    public void PlayReleaseObject() => PlayClip(releaseObject);
    public void PlayTackObject() => PlayClip(tackObject);
    public void PlayKick() => PlayClip(kick);
    public void PlayDestroyBarricade() => PlayClip(destroyBarricade);

    // ------- Helpers -------
    private void PlayRandomFromArray(AudioClip[] clips)
    {
        if (clips.Length == 0) return;
        int index = Random.Range(0, clips.Length);
        PlayClip(clips[index]);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
