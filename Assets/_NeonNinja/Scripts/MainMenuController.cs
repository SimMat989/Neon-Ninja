using UnityEngine;
using UnityEngine.SceneManagement; // Fondamentale per cambiare scena
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI highScoreText;
    
    [Header("Scene Settings")]
    public string gameSceneName = "GameScene"; // <-- IMPORTANTE: Scrivi qui il nome esatto della tua scena di gioco

    private void Start()
    {
        // Assicuriamoci che il tempo scorra normalmente nel menu
        Time.timeScale = 1f;

        // Leggiamo l'High Score direttamente dai salvataggi (stessa chiave del SaveManager)
        int score = PlayerPrefs.GetInt("HighScore", 0);
        
        if (highScoreText != null) 
        {
            highScoreText.text = $"Record: {score}";
        }
    }

    // Collega questo metodo all'evento OnClick del bottone "Gioca"
    public void PlayGame()
    {
        // Carica la scena del livello
        SceneManager.LoadScene(gameSceneName);
    }
    
    // Opzionale: un bottone per uscire dal gioco
    public void QuitGame()
    {
        Debug.Log("Uscita dal gioco...");
        Application.Quit();
    }
}