using UnityEngine;

public class SizeManager : MonoBehaviour
{
    public static SizeManager Instance { get; private set; }

    [Header("Settings")]
    public int maxGrowth = 5;
    public float scaleStep = 0.2f; // 20% per ogni stadio

    public int CurrentStage { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Modifica la dimensione del player. 
    /// +1 per Salto (Ingrandisce), -1 per Scatto (Rimpicciolisce)
    /// </summary>
    public void ChangeSize(int amount, Transform playerTransform)
    {
        CurrentStage += amount;

        // Controllo condizioni di sconfitta
        if (CurrentStage >= maxGrowth)
        {
            GameManager.Instance.GameOver("Sei esploso! (Troppo grande)");
            return;
        }
        if (CurrentStage <= -maxGrowth)
        {
            GameManager.Instance.GameOver("Sei svanito! (Troppo piccolo)");
            return;
        }

        // Applica la nuova scala
        float newScale = 1f + (CurrentStage * scaleStep);
        // Mantiene Z a 1
        playerTransform.localScale = new Vector3(newScale, newScale, 1f);
        
        // Effetto sonoro (opzionale)
        if(amount > 0) AudioManager.Instance.PlaySound("Grow");
        else AudioManager.Instance.PlaySound("Shrink");
    }

    /// <summary>
    /// Restituisce la forza del salto basata sulla dimensione.
    /// Più sei grande, meno salti.
    /// </summary>
    public float GetJumpMultiplier()
    {
        float scale = 1f + (CurrentStage * scaleStep);
        // Inverso proporzionale: più grande è la scala, minore è il moltiplicatore
        return 1f / scale;
    }
}