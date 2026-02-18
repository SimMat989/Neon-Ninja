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
    public AudioClip growClip;    // Attualmente inutilizzato, ma pronto all'uso!
    public AudioClip shrinkClip;  // Attualmente inutilizzato, ma pronto all'uso!
    public AudioClip gameOverClip;

    [Header("Pitch Settings")]
    public float normalPitch = 1.0f; 
    public float minPitch = 0.6f;    // Grave (Grande)
    public float maxPitch = 1.5f;    // Acuto (Piccolo)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
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
            default:
                Debug.LogWarning("Attenzione: nessun suono trovato con il nome '" + soundName + "'");
                break;
        }
    }

    public void UpdatePitchByLevel(int currentLevel, int maxLevel)
    {
        if (musicSource == null) return;

        if (currentLevel > 0)
        {
            float percent = (float)currentLevel / maxLevel;
            musicSource.pitch = Mathf.Lerp(normalPitch, minPitch, percent);
        }
        else if (currentLevel < 0)
        {
            float percent = (float)Mathf.Abs(currentLevel) / maxLevel;
            musicSource.pitch = Mathf.Lerp(normalPitch, maxPitch, percent);
        }
        else
        {
            musicSource.pitch = normalPitch;
        }
    }
}