using UnityEngine;
using TMPro; // Assicurati di aver importato TMP Essentials in Unity

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject gameHUD;
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
        gameOverPanel.SetActive(false);
    }

    public void ShowHUD()
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void UpdateScoreUI(int score)
    {
        scoreText.text = $"Punti: {score}";
    }

    public void ShowGameOver(string reason, int score)
    {
        menuPanel.SetActive(false);
        gameHUD.SetActive(false);
        gameOverPanel.SetActive(true);

        endReasonText.text = reason;
        endScoreText.text = $"Punteggio Finale: {score}";
    }

    // Collegare questo metodo al bottone "Gioca" nell'Inspector
    public void OnPlayButton()
    {
        GameManager.Instance.StartGame();
    }

    // Collegare questo metodo al bottone "Riprova" nell'Inspector
    public void OnRestartButton()
    {
        GameManager.Instance.RestartGame();
    }
}