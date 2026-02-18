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

    private float targetScale = 1f;
    private Transform playerTransformReference; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // Gestisce l'animazione fluida della scala
        if (playerTransformReference != null)
        {
            float currentScale = playerTransformReference.localScale.x;

            if (Mathf.Abs(currentScale - targetScale) > 0.001f)
            {
                float newScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * lerpSpeed);
                playerTransformReference.localScale = new Vector3(newScale, newScale, 1f);
            }
        }
    }

    public void ChangeSize(int amount, Transform playerTransform)
    {
        playerTransformReference = playerTransform;
        CurrentStage += amount;

        // 1. Controllo Game Over
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

        // 2. Calcola la nuova scala bersaglio per il Lerp
        targetScale = 1f + (CurrentStage * scaleStep);
        targetScale = Mathf.Max(0.1f, targetScale); 

        // 3. Modifica la musica di sottofondo
        AudioManager.Instance.UpdatePitchByLevel(CurrentStage, maxGrowth);
    }

    public float GetJumpMultiplier()
    {
        float scale = 1f + (CurrentStage * scaleStep);
        scale = Mathf.Max(0.1f, scale); 
        return 1f / scale;
    }
}