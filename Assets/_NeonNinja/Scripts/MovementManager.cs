using UnityEngine;
using System.Collections; // Necessario per le Coroutine

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

    private bool _canMove = true; // Impostato a true di base per testare subito
    private bool _isDashing = false;
    private int _jumpCount = 0;
    private const int MaxJumps = 2;
    
    private Camera _mainCamera; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _mainCamera = Camera.main; 
    }

    private void Update()
    {
        if (!_canMove || GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        HandleInput();
        CheckBounds();
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

        // Salto (Space)
        if (Input.GetKeyDown(KeyCode.Space) && !_isDashing)
        {
            if (IsGrounded()) 
            {
                _jumpCount = 0;
            }

            if (IsGrounded() || _jumpCount < MaxJumps)
            {
                PerformJump();
            }
        }

        // Scatto (Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (!_isDashing) 
            {
                StartCoroutine(PerformDash(xInput));
            }
        }
    }

    private void PerformJump()
    {
        _jumpCount++;
        
        // Chiede la forza del salto al SizeManager in base alle dimensioni attuali
        float multiplier = SizeManager.Instance.GetJumpMultiplier();
        float finalForce = baseJumpForce * multiplier;

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0); 
        playerRb.AddForce(Vector2.up * finalForce, ForceMode2D.Impulse);

        // AVVISA IL GAME MANAGER
        GameManager.Instance.OnPlayerJump(playerTransform);
    }

    private IEnumerator PerformDash(float direction)
    {
        _isDashing = true;
        
        float originalGravity = playerRb.gravityScale;
        playerRb.gravityScale = 0; 
        playerRb.linearVelocity = Vector2.zero; 

        float dashDir = direction != 0 ? direction : 1f;
        playerRb.linearVelocity = new Vector2(dashDir * dashSpeed, 0);

        // AVVISA IL GAME MANAGER
        GameManager.Instance.OnPlayerDash(playerTransform);

        yield return new WaitForSeconds(dashDuration);

        playerRb.gravityScale = originalGravity;
        _isDashing = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundLayer);
    }

    private void CheckBounds()
    {
        if (playerTransform.position.y < -10f)
        {
            GameManager.Instance.GameOver("Sei caduto nel vuoto!");
        }

        Vector3 viewPos = _mainCamera.WorldToViewportPoint(playerTransform.position);
        if (viewPos.x < 0)
        {
            GameManager.Instance.GameOver("Troppo lento!");
        }
    }
}