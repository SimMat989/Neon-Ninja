using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Menu, Playing, GameOver }
    public GameState CurrentState { get; private set; }

    public float Score { get; private set; }
    private float _scoreMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CurrentState = GameState.Menu;
    }

    private void Start()
    {
        // Inizializza la UI all'avvio
        UIManager.Instance.ShowMenu();
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            // Aumenta punteggio basato sulla velocità della camera
            Score += LevelManager.Instance.cameraSpeed * Time.deltaTime * 10f;
            UIManager.Instance.UpdateScoreUI(Mathf.FloorToInt(Score));
        }
    }

    public void StartGame()
    {
        Score = 0;
        CurrentState = GameState.Playing;
        
        // Notifica gli altri manager
        LevelManager.Instance.StartLevelGeneration();
        MovementManager.Instance.EnableMovement(true);
        UIManager.Instance.ShowHUD();
        
        Debug.Log("Gioco Iniziato!");
    }

    public void GameOver(string reason)
    {
        if (CurrentState == GameState.GameOver) return;

        CurrentState = GameState.GameOver;
        MovementManager.Instance.EnableMovement(false);
        
        // Salva il punteggio
        SaveManager.Instance.SaveHighScore(Mathf.FloorToInt(Score));
        
        // Mostra schermata finale
        UIManager.Instance.ShowGameOver(reason, Mathf.FloorToInt(Score));
        Debug.Log($"Game Over: {reason}");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}