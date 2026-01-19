using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Clips")]
    public AudioClip jumpClip;
    public AudioClip dashClip;
    public AudioClip growClip;
    public AudioClip shrinkClip;
    public AudioClip gameOverClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // L'audio persiste tra le scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string soundName)
    {
        switch (soundName)
        {
            case "Jump":
                sfxSource.PlayOneShot(jumpClip);
                break;
            case "Dash":
                sfxSource.PlayOneShot(dashClip);
                break;
            case "Grow":
                sfxSource.PlayOneShot(growClip);
                break;
            case "Shrink":
                sfxSource.PlayOneShot(shrinkClip);
                break;
            case "GameOver":
                sfxSource.PlayOneShot(gameOverClip);
                break;
        }
    }
}