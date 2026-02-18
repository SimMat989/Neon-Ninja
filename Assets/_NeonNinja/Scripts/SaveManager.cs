using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            // Opzionale: se hai più scene in futuro, questo lo terrà in vita
            DontDestroyOnLoad(gameObject); 
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Tenta di salvare il punteggio. Ritorna TRUE se è un nuovo record.
    /// </summary>
    public bool SaveHighScore(int score)
    {
        int currentHigh = GetHighScore();
        if (score > currentHigh)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log("Nuovo Record Salvato: " + score);
            return true; // Evviva, nuovo record!
        }
        
        return false; // Nessun nuovo record
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt(HighScoreKey, 0); // 0 è il valore di default se non c'è salvataggio
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey(HighScoreKey); // Più sicuro di DeleteAll()
        Debug.Log("Dati High Score resettati.");
    }
}