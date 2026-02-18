using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject gameHUD;
    public GameObject pausePanel; // AGGIUNTO: Pannello della pausa
    public GameObject gameOverPanel;

    [Header("Text Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endScoreText;
    public TextMeshProUGUI endReasonText;

    [Header("High Score Text")]
    public TextMeshProUGUI menuHighScoreText; // Aggiungi questa variabile
    public GameObject newRecordAlert;         // Aggiungi un testo/immagine "NUOVO RECORD!" disattivato di base

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        gameHUD.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowHUD()
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(true);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Viene chiamato dal GameManager quando si preme Esc o P
    /// </summary>
    public void TogglePauseMenu(bool isPaused)
    {
        // Accende o spegne il pannello della pausa. 
        // Nota: lasciamo il gameHUD acceso così si vede in sottofondo!
        pausePanel.SetActive(isPaused);
    }

    public void UpdateScoreUI(int score)
    {
        scoreText.text = $"Punti: {score}";
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        gameHUD.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // MOSTRA L'HIGH SCORE NEL MENU
        if (SaveManager.Instance != null)
        {
            menuHighScoreText.text = $"Record: {SaveManager.Instance.GetHighScore()}";
        }
    }

    // Aggiorniamo la firma del metodo per ricevere il booleano isNewRecord
    public void ShowGameOver(string reason, int score, bool isNewRecord)
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        endReasonText.text = reason;
        endScoreText.text = $"Punteggio Finale: {score}";

        // ACCENDE IL TESTO "NUOVO RECORD" SE SERVE
        if (newRecordAlert != null)
        {
            newRecordAlert.SetActive(isNewRecord);
        }
    }

    // --- METODI PER I BOTTONI DELL'INTERFACCIA ---

    // Da collegare al bottone "Gioca" nel Menu Iniziale
    public void OnPlayButton()
    {
        GameManager.Instance.StartGame();
    }

    // Da collegare al bottone "Riprendi" nel Pannello di Pausa
    public void OnResumeButton()
    {
        // Diciamo al GameManager di togliere la pausa (lui poi richiamerà TogglePauseMenu(false))
        GameManager.Instance.TogglePause();
    }

    // Da collegare al bottone "Riprova" nel Pannello di Game Over
    public void OnRestartButton()
    {
        GameManager.Instance.RestartGame();
    }
}