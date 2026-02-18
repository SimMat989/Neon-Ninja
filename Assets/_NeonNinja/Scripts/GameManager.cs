using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // 1. Aggiunto lo stato "Paused"
    public enum GameState { Menu, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // 2. Controllo dell'input per la pausa
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            // Possiamo mettere in pausa solo se stiamo giocando, 
            // o toglierla se siamo già in pausa (non se siamo in GameOver)
            if (CurrentState == GameState.Playing || CurrentState == GameState.Paused)
            {
                TogglePause();
            }
        }
    }

    /// <summary>
    /// Attiva o disattiva la pausa
    /// </summary>
    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            
            // Ferma il tempo del motore fisico e delle animazioni
            Time.timeScale = 0f; 
            
            // Abbassa il volume della musica o mettila in pausa (opzionale)
            if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
                AudioManager.Instance.musicSource.Pause();

            // Richiama l'UIManager per mostrare la grafica (lo creeremo dopo)
            // UIManager.Instance.TogglePauseMenu(true); 
            
            Debug.Log("Gioco in Pausa");
        }
        else if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            
            // Ripristina lo scorrere del tempo
            Time.timeScale = 1f; 
            
            // Fai ripartire la musica
            if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
                AudioManager.Instance.musicSource.UnPause();

            // Nascondi il menu di pausa
            // UIManager.Instance.TogglePauseMenu(false); 
            
            Debug.Log("Gioco Ripreso");
        }
    }

    // --- ALTRI METODI GIA' PRESENTI ---

    public void OnPlayerJump(Transform playerTransform)
    {
        AudioManager.Instance.PlaySound("Jump");
        SizeManager.Instance.ChangeSize(1, playerTransform);
    }

    public void OnPlayerDash(Transform playerTransform)
    {
        AudioManager.Instance.PlaySound("Dash");
        SizeManager.Instance.ChangeSize(-1, playerTransform);
    }

    public void GameOver(string reason)
    {
        CurrentState = GameState.GameOver;
        Time.timeScale = 0f; // Opzionale: ferma tutto anche al Game Over

        Debug.Log("GAME OVER: " + reason);
        AudioManager.Instance.PlaySound("GameOver");
        
        if (MovementManager.Instance != null)
        {
            MovementManager.Instance.EnableMovement(false);
        }
        
        // UIManager.Instance.ShowGameOverScreen();
    }
}