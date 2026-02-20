using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject gameHUD;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Text Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI endScoreText;
    public TextMeshProUGUI endReasonText;
    
    [Header("High Score UI")]
    public TextMeshProUGUI menuHighScoreText; 
    public GameObject newRecordAlert; // L'avviso "NUOVO RECORD!"

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

        if (SaveManager.Instance != null && menuHighScoreText != null)
        {
            menuHighScoreText.text = $"Record: {SaveManager.Instance.GetHighScore()}";
        }
    }

    public void ShowHUD()
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(true);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void TogglePauseMenu(bool isPaused)
    {
        pausePanel.SetActive(isPaused);
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null) scoreText.text = $"Punti: {score}";
    }

    public void ShowGameOver(string reason, int score, bool isNewRecord)
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        if (endReasonText != null) endReasonText.text = reason;
        if (endScoreText != null) endScoreText.text = $"Punteggio Finale: {score}";

        if (newRecordAlert != null)
        {
            newRecordAlert.SetActive(isNewRecord);
        }
    }

    // --- METODI PER I BOTTONI ---

    public void OnPlayButton()
    {
        if (GameManager.Instance != null) GameManager.Instance.StartGame();
    }

    public void OnResumeButton()
    {
        if (GameManager.Instance != null) GameManager.Instance.TogglePause();
    }

    public void OnRestartButton()
    {
        if (GameManager.Instance != null) GameManager.Instance.RestartGame();
    }

    // AGGIUNGI QUESTO NUOVO METODO:
    public void OnMainMenuButton()
    {
        if (GameManager.Instance != null) GameManager.Instance.GoToMainMenu();
    }
}