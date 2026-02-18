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

    public void ShowGameOver(string reason, int score)
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        endReasonText.text = reason;
        endScoreText.text = $"Punteggio Finale: {score}";
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