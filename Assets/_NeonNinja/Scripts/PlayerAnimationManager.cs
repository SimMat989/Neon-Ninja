using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    public static PlayerAnimationManager Instance { get; private set; }

    [Header("References")]
    public SpriteRenderer spriteRenderer; 
    public Rigidbody2D playerRb;

    [Header("Animation Frames (Sprites)")]
    public Sprite[] idleFrames;  // AGGIUNTO: I frame per quando sei fermo
    public Sprite[] runFrames;   
    public Sprite jumpFrame;     
    public Sprite fallFrame;     
    public Sprite dashFrame;     

    [Header("Settings")]
    public float idleFrameRate = 8f; // L'idle di solito è un po' più lenta della corsa
    public float runFrameRate = 12f; 

    private int _currentFrame;
    private float _timer;
    private bool _isDashing;
    
    // Serve per capire se stiamo cambiando animazione (es. da Idle a Run)
    private Sprite[] _currentAnimArray; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // 1. PRIORITÀ ASSOLUTA: Scatto
        if (_isDashing)
        {
            if (dashFrame != null) spriteRenderer.sprite = dashFrame;
            return; 
        }

        // 2. Lettura dello stato fisico
        bool isGrounded = false;
        if (MovementManager.Instance != null)
        {
            isGrounded = MovementManager.Instance.IsGrounded();
        }
        
        float yVel = playerRb.linearVelocity.y;
        float xVel = Mathf.Abs(playerRb.linearVelocity.x); // Velocità orizzontale assoluta (senza segno meno)

        // 3. Logica di scambio Sprite
        if (isGrounded)
        {
            // Se siamo a terra, controlliamo se ci stiamo muovendo!
            if (xVel > 0.1f)
            {
                // Stiamo correndo
                PlayAnimation(runFrames, runFrameRate);
            }
            else
            {
                // Siamo fermi
                PlayAnimation(idleFrames, idleFrameRate);
            }
        }
        else
        {
            // Se siamo in aria, controlliamo se saliamo o scendiamo
            // (E resettiamo l'array corrente, così quando atterriamo la corsa riparte da zero)
            _currentAnimArray = null;

            if (yVel > 0.1f && jumpFrame != null)
            {
                spriteRenderer.sprite = jumpFrame;
            }
            else if (yVel < -0.1f && fallFrame != null)
            {
                spriteRenderer.sprite = fallFrame;
            }
        }
    }

    /// <summary>
    /// Metodo universale per gestire sia la Corsa che l'Idle in loop.
    /// </summary>
    private void PlayAnimation(Sprite[] frames, float frameRate)
    {
        if (frames == null || frames.Length == 0) return;

        // Se l'animazione è cambiata (es. da fermo ho iniziato a correre),
        // resettiamo il timer e il frame per farla partire dall'inizio pulita.
        if (_currentAnimArray != frames)
        {
            _currentAnimArray = frames;
            _currentFrame = 0;
            _timer = 0f;
        }

        _timer += Time.deltaTime;

        if (_timer >= 1f / frameRate)
        {
            _timer -= 1f / frameRate; 
            _currentFrame = (_currentFrame + 1) % frames.Length;
        }
        
        // Applichiamo la nuova immagine al personaggio!
        spriteRenderer.sprite = frames[_currentFrame];
    }

    // --- METODI CHIAMATI DAL GAME MANAGER ---

    public void PlayDash()
    {
        _isDashing = true;
        
        float duration = 0.2f; 
        if (MovementManager.Instance != null) duration = MovementManager.Instance.dashDuration;

        Invoke(nameof(StopDash), duration);
    }

    private void StopDash()
    {
        _isDashing = false;
    }
}