using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    public static PlayerAnimationManager Instance { get; private set; }

    [Header("References")]
    public SpriteRenderer spriteRenderer; 
    public Rigidbody2D playerRb;

    [Header("Animation Frames (Sprites)")]
    public Sprite[] idleFrames;  
    public Sprite[] runFrames;   
    public Sprite jumpFrame;     
    public Sprite fallFrame;     
    public Sprite dashFrame;     

    [Header("Settings")]
    public float idleFrameRate = 8f; 
    public float runFrameRate = 12f; 

    private int _currentFrame;
    private float _timer;
    private bool _isDashing;
    
    private Sprite[] _currentAnimArray; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // --- FLIP (Capovolgimento a sinistra o destra) ---
        // Leggiamo la velocità REALE sull'asse X (con il segno negativo se andiamo a sinistra)
        float realXVel = playerRb.linearVelocity.x;

        if (realXVel > 0.1f)
        {
            spriteRenderer.flipX = false; // Guarda a destra
        }
        else if (realXVel < -0.1f)
        {
            spriteRenderer.flipX = true;  // Guarda a sinistra (Flippato!)
        }

        // --- 1. PRIORITÀ ASSOLUTA: Scatto ---
        if (_isDashing)
        {
            if (dashFrame != null) 
            {
                spriteRenderer.sprite = dashFrame;
            }
            return; // Blocca tutto! Il salto e la caduta vengono ignorati finché scatti.
        }

        // --- 2. Lettura dello stato fisico ---
        bool isGrounded = false;
        if (MovementManager.Instance != null)
        {
            isGrounded = MovementManager.Instance.IsGrounded();
        }
        
        float yVel = playerRb.linearVelocity.y;
        float absXVel = Mathf.Abs(realXVel); // Velocità orizzontale assoluta (senza segno meno)

        // --- 3. Logica di scambio Sprite ---
        if (isGrounded)
        {
            if (absXVel > 0.1f)
            {
                PlayAnimation(runFrames, runFrameRate);
            }
            else
            {
                PlayAnimation(idleFrames, idleFrameRate);
            }
        }
        else
        {
            _currentAnimArray = null; // Reset dell'animazione per quando atterreremo

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

    private void PlayAnimation(Sprite[] frames, float frameRate)
    {
        if (frames == null || frames.Length == 0) return;

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