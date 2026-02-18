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

    [Header("Pitch Settings")]
    public float normalPitch = 1.0f; // Pitch a grandezza 0
    public float minPitch = 0.6f;    // Pitch a grandezza +4 (quasi Game Over)
    public float maxPitch = 1.5f;    // Pitch a grandezza -4 (quasi Game Over)

    /// <summary>
    /// Viene chiamato dal SizeManager ogni volta che la grandezza cambia.
    /// </summary>
    public void UpdatePitchByLevel(int currentLevel, int maxLevel)
    {
        if (musicSource == null) return;

        if (currentLevel > 0)
        {
            // Il player è GRANDE. Calcoliamo la percentuale verso il game over.
            // Es: Se currentLevel è 2 e maxLevel è 5 -> percentuale = 2/5 = 0.4 (40%)
            float percent = (float)currentLevel / maxLevel;
            
            // Spostiamo il pitch verso il grave (minPitch) in base alla percentuale
            musicSource.pitch = Mathf.Lerp(normalPitch, minPitch, percent);
        }
        else if (currentLevel < 0)
        {
            // Il player è PICCOLO. 
            float percent = (float)Mathf.Abs(currentLevel) / maxLevel;
            
            // Spostiamo il pitch verso l'acuto (maxPitch) in base alla percentuale
            musicSource.pitch = Mathf.Lerp(normalPitch, maxPitch, percent);
        }
        else
        {
            // Il player è tornato alla grandezza NORMALE (livello 0)
            musicSource.pitch = normalPitch;
        }
    }

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
            default:
                // Se scrivi male il nome del suono, Unity te lo segnalerà nella console!
                Debug.LogWarning("Attenzione: nessun suono trovato con il nome '" + soundName + "'");
                break;
        }
    }

    /// <summary>
    /// Cambia il pitch della musica in base alla dimensione.
    /// Viene chiamato dal SizeManager.
    /// </summary>
    public void UpdatePitchBasedOnSize(float sizeNormalized)
    {
        if (musicSource != null)
        {
            // Calcola il pitch in base alla percentuale (0 = piccolo/acuto, 1 = grande/grave)
            float targetPitch = Mathf.Lerp(maxPitch, minPitch, sizeNormalized);
            musicSource.pitch = targetPitch;
        }
    }
}