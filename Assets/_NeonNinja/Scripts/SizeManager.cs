using UnityEngine;

public class SizeManager : MonoBehaviour
{
    public static SizeManager Instance { get; private set; }

    [Header("Size Settings")]
    public int maxGrowth = 5;
    public float scaleStep = 0.2f; // 20% per ogni stadio
    
    [Header("Lerp Settings")]
    [Tooltip("Velocità con cui il personaggio raggiunge la nuova dimensione")]
    public float lerpSpeed = 8f; 

    public int CurrentStage { get; private set; } = 0;

    // Variabili per gestire l'interpolazione fluida
    private float targetScale = 1f;
    private Transform playerTransformReference; // Salviamo il riferimento al player

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // Se abbiamo un riferimento al player e la sua scala non ha ancora raggiunto il target
        if (playerTransformReference != null)
        {
            float currentScale = playerTransformReference.localScale.x;

            // Se la differenza tra la scala attuale e il target è rilevante, continuiamo il Lerp
            if (Mathf.Abs(currentScale - targetScale) > 0.001f)
            {
                // Calcola il passo di interpolazione per questo frame
                float newScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * lerpSpeed);
                
                // Applica la nuova scala
                playerTransformReference.localScale = new Vector3(newScale, newScale, 1f);
            }
        }
    }

    /// <summary>
    /// Modifica la dimensione del player. 
    /// +1 per Salto (Ingrandisce), -1 per Scatto (Rimpicciolisce)
    /// </summary>
    public void ChangeSize(int amount, Transform playerTransform)
    {
        // Salviamo il riferimento al transform del player per poterlo usare nell'Update
        playerTransformReference = playerTransform;
        
        CurrentStage += amount;

        // 1. Controllo condizioni di sconfitta
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

        // 2. Calcola la NUOVA SCALA BERSAGLIO (invece di applicarla subito)
        targetScale = 1f + (CurrentStage * scaleStep);
        targetScale = Mathf.Max(0.1f, targetScale); // Sicurezza

        // 3. Effetti Sonori (SFX)
        if (amount > 0) 
            AudioManager.Instance.PlaySound("Grow");
        else if (amount < 0) 
            AudioManager.Instance.PlaySound("Shrink");

        // 4. Aggiorna il Pitch della musica di sottofondo
        AudioManager.Instance.UpdatePitchByLevel(CurrentStage, maxGrowth);
    }

    /// <summary>
    /// Restituisce la forza del salto basata sulla dimensione.
    /// Più sei grande, meno salti.
    /// </summary>
    public float GetJumpMultiplier()
    {
        // Usiamo il CurrentStage per il calcolo della fisica, in modo che 
        // l'effetto sul salto sia immediato anche se l'animazione visiva (Lerp) è in corso.
        float scale = 1f + (CurrentStage * scaleStep);
        scale = Mathf.Max(0.1f, scale); 
        return 1f / scale;
    }
}