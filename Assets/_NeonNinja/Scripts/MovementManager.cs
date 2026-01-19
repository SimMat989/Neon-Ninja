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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!_canMove || GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        HandleInput();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded() || _jumpCount < MaxJumps)
            {
                PerformJump();
            }
        }

        // Scatto (Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if (!_isDashing) StartCoroutine(PerformDash(xInput));
        }
        
        // Controllo Morte (Caduta o fuori camera)
        CheckBounds();
    }

    private void PerformJump()
    {
        _jumpCount++;
        
        // Richiedi moltiplicatore al SizeManager
        float multiplier = SizeManager.Instance.GetJumpMultiplier();
        float finalForce = baseJumpForce * multiplier;

        // Reset velocità verticale per salto consistente
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0); 
        playerRb.AddForce(Vector2.up * finalForce, ForceMode2D.Impulse);

        // Effetto collaterale: Ingrandimento
        SizeManager.Instance.ChangeSize(1, playerTransform);
        AudioManager.Instance.PlaySound("Jump");
    }

    private System.Collections.IEnumerator PerformDash(float direction)
    {
        _isDashing = true;
        float originalGravity = playerRb.gravityScale;
        playerRb.gravityScale = 0; // Gravità zero durante il dash

        // Se non c'è input, scatta nella direzione in cui guarda (o destra di default)
        float dashDir = direction != 0 ? direction : 1f;
        playerRb.linearVelocity = new Vector2(dashDir * dashSpeed, 0);

        // Effetto collaterale: Rimpicciolimento
        SizeManager.Instance.ChangeSize(-1, playerTransform);
        AudioManager.Instance.PlaySound("Dash");

        yield return new WaitForSeconds(dashDuration);

        playerRb.gravityScale = originalGravity;
        _isDashing = false;
    }

    private bool IsGrounded()
    {
        // Semplice raycast verso il basso
        bool hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.2f, groundLayer);
        if (hit) _jumpCount = 0; // Reset salti
        return hit;
    }

    private void CheckBounds()
    {
        // Se cade troppo in basso
        if (playerTransform.position.y < -10f)
        {
            GameManager.Instance.GameOver("Sei caduto nel vuoto!");
        }

        // Se tocca il bordo sinistro della camera
        Vector3 viewPos = Camera.main.WorldToViewportPoint(playerTransform.position);
        if (viewPos.x < 0)
        {
            GameManager.Instance.GameOver("Troppo lento!");
        }
    }
}