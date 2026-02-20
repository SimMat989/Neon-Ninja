using UnityEngine;
using UnityEngine.SceneManagement; // Necessario per ricaricare la scena
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Stati di gioco
    public enum GameState { Menu, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Menu;

    [Header("Score Settings")]
    public float scoreMultiplier = 10f; // Quanti punti guadagni al secondo
    private float currentScore = 0f;

    private void Awake()
    {
        // Setup del Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Non partiamo più dal Menu, ma avviamo direttamente il gioco!
        // StartGame() rimetterà il Time.timeScale a 1 e cambierà lo stato in Playing.
        StartGame(); 
        
        // Diciamo al LevelManager di iniziare a generare le piattaforme subito
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartLevelGeneration();
        }
    }

    private void Update()
    {
        // 1. Gestione del punteggio automatico (tipico degli autoscroller)
        if (CurrentState == GameState.Playing)
        {
            currentScore += Time.deltaTime * scoreMultiplier;
            
            if (UIManager.Instance != null)
            {
                // Convertiamo in int per evitare i decimali nell'interfaccia
                UIManager.Instance.UpdateScoreUI(Mathf.FloorToInt(currentScore));
            }
        }

        // 2. Controllo dell'input per la Pausa
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (CurrentState == GameState.Playing || CurrentState == GameState.Paused)
            {
                TogglePause();
            }
        }
    }

    // --- METODI PER IL FLUSSO DI GIOCO ---

    /// <summary>
    /// Chiamato dall'UI Manager (Bottone "Gioca" nel menu principale)
    /// </summary>
    public void StartGame()
    {
        CurrentState = GameState.Playing;
        currentScore = 0f;
        Time.timeScale = 1f;

        // NUOVO: Nascondi e blocca il cursore mentre giochi!
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowHUD();
            UIManager.Instance.UpdateScoreUI(0);
        }
        
        if (LevelManager.Instance != null) LevelManager.Instance.StartLevelGeneration();
    }

    /// <summary>
    /// Riavvia la scena attuale (Il livello di gioco)
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // Sblocca il tempo!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Torna alla scena del Menu Principale
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Sblocca il tempo!
        // ATTENZIONE: Se la tua scena del menu si chiama in un altro modo, cambialo qui!
        SceneManager.LoadScene("0_MainMenu"); 
    }

    /// <summary>
    /// Attiva o disattiva la pausa
    /// </summary>
    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            Time.timeScale = 0f; 
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
                AudioManager.Instance.musicSource.Pause();

            if (UIManager.Instance != null)
                UIManager.Instance.TogglePauseMenu(true); 
        }
        else if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f; 
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // --- NUOVA RIGA: Togliamo il focus dal bottone della UI! ---
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
                AudioManager.Instance.musicSource.UnPause();

            if (UIManager.Instance != null)
                UIManager.Instance.TogglePauseMenu(false); 
        }
    }

    // --- METODI PER LE AZIONI DEL GIOCATORE ---

    public void OnPlayerJump(Transform playerTransform)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySound("Jump");
        if (SizeManager.Instance != null) SizeManager.Instance.ChangeSize(1, playerTransform);
    }

    public void OnPlayerDash(Transform playerTransform)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySound("Dash");
        if (SizeManager.Instance != null) SizeManager.Instance.ChangeSize(-1, playerTransform);
        
        // QUESTA RIGA È FONDAMENTALE PER FAR PARTIRE L'ANIMAZIONE
        if (PlayerAnimationManager.Instance != null) PlayerAnimationManager.Instance.PlayDash();
    }

    // --- SCONFITTA ---

    public void GameOver(string reason)
    {
        if (CurrentState == GameState.GameOver) return; 

        CurrentState = GameState.GameOver;
        Time.timeScale = 0f; 

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySound("GameOver");
        if (MovementManager.Instance != null) MovementManager.Instance.EnableMovement(false);
        
        // --- NUOVA PARTE PER IL SALVATAGGIO ---
        int finalScore = Mathf.FloorToInt(currentScore);
        
        bool isNewRecord = false;
        if (SaveManager.Instance != null)
        {
            // Salva e scopri se è un nuovo record
            isNewRecord = SaveManager.Instance.SaveHighScore(finalScore);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(reason, finalScore, isNewRecord);
        }
    }
}