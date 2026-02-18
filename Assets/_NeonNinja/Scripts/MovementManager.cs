using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public static MovementManager Instance { get; private set; }

    [Header("Player Settings")]
    public float moveSpeed = 8f;
    public float baseJumpForce = 12f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;

    [Header("References")]
    public Rigidbody2D playerRb;
    public Transform playerTransform;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private bool _canMove = false;
    private bool _isDashing = false;
    private int _jumpCount = 0;
    private const int MaxJumps = 2;
    
    // Ottimizzazione: salviamo la telecamera
    private Camera _mainCamera; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Troviamo la telecamera una volta sola all'avvio
        _mainCamera = Camera.main; 
    }

    private void Update()
    {
        if (!_canMove || GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        HandleInput();
        CheckBounds(); // Separato per maggiore pulizia
    }

    public void EnableMovement(bool enable)
    {
        _canMove = enable;
        if (!enable && playerRb != null) playerRb.linearVelocity = Vector2.zero;
    }

    private void HandleInput()
    {
        // Movimento Orizzontale (A/D)
        float xInput = Input.GetAxisRaw("Horizontal");
        
        if (!_isDashing)
        {
            playerRb.linearVelocity = new Vector2(xInput * moveSpeed, playerRb.linearVelocity.y);
        }

        // Salto (Space) - AGGIUNTO: Non puoi saltare mentre fai uno scatto
        if (Input.GetKeyDown(KeyCode.Space) && !_isDashing)
        {
            // Controlla se siamo a terra e in caso resetta i salti
            if (IsGrounded()) 
            {
                _jumpCount = 0;
            }

            // Se siamo a terra o abbiamo ancora salti disponibili
            if (IsGrounded() || _jumpCount < MaxJumps)
            {
                PerformJump();
            }
        }

        // Scatto (Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            // AGGIUNTO: Non puoi scattare se lo stai già facendo
            if (!_isDashing) 
            {
                StartCoroutine(PerformDash(xInput));
            }
        }
    }

    private void PerformJump()
    {
        _jumpCount++;
        
        // Richiedi moltiplicatore al SizeManager
        float multiplier = SizeManager.Instance.GetJumpMultiplier();
        float finalForce = baseJumpForce * multiplier;

        // Reset velocità verticale per evitare salti "flosci" se si stava cadendo
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0); 
        playerRb.AddForce(Vector2.up * finalForce, ForceMode2D.Impulse);

        // Integrazione
        SizeManager.Instance.ChangeSize(1, playerTransform);
    }

    private System.Collections.IEnumerator PerformDash(float direction)
    {
        _isDashing = true;
        
        float originalGravity = playerRb.gravityScale;
        playerRb.gravityScale = 0; // Gravità zero durante il dash

        // Reset della velocità verticale per evitare di scattare in diagonale se si stava cadendo
        playerRb.linearVelocity = Vector2.zero; 

        // Se non c'è input, scatta a destra di default
        float dashDir = direction != 0 ? direction : 1f;
        playerRb.linearVelocity = new Vector2(dashDir * dashSpeed, 0);

        // Integrazione
        SizeManager.Instance.ChangeSize(-1, playerTransform);

        yield return new WaitForSeconds(dashDuration);

        // Ripristino post-scatto
        playerRb.gravityScale = originalGravity;
        _isDashing = false;
    }

    private bool IsGrounded()
    {
        // Raycast verso il basso. (Nota: assicurati che il Layer del player NON sia "groundLayer")
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundLayer);
    }

    private void CheckBounds()
    {
        // Se cade troppo in basso
        if (playerTransform.position.y < -10f)
        {
            GameManager.Instance.GameOver("Sei caduto nel vuoto!");
        }

        // Se tocca il bordo sinistro della camera
        Vector3 viewPos = _mainCamera.WorldToViewportPoint(playerTransform.position);
        if (viewPos.x < 0)
        {
            GameManager.Instance.GameOver("Troppo lento!");
        }
    }
}