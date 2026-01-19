using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float cameraSpeed = 3f;
    public float speedIncreaseRate = 0.1f;

    [Header("Generation Settings")]
    public GameObject platformPrefab;
    public Transform generationPoint; // Punto davanti alla camera dove generare
    public float minGap = 2f;
    public float maxGap = 5f;
    public float widthMultiplier = 1f;

    private float _distanceTraveled;
    private Vector3 _lastPlatformEndPosition;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartLevelGeneration()
    {
        _lastPlatformEndPosition = new Vector3(-5, -2, 0); // Start position
        SpawnPlatform(10f); // Piattaforma iniziale lunga
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // Muovi la camera
        cameraTransform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
        
        // Aumenta difficoltà
        cameraSpeed += speedIncreaseRate * Time.deltaTime;

        // Genera piattaforme
        if (Vector3.Distance(_lastPlatformEndPosition, generationPoint.position) < 20f)
        {
            GenerateNextPlatform();
        }
    }

    private void GenerateNextPlatform()
    {
        float gap = Random.Range(minGap, maxGap);
        float heightChange = Random.Range(-1.5f, 1.5f);
        float width = Random.Range(2f, 6f);

        Vector3 spawnPos = _lastPlatformEndPosition + new Vector3(gap + (width/2), heightChange, 0);
        
        // Mantieni la Y entro limiti giocabili
        spawnPos.y = Mathf.Clamp(spawnPos.y, -4f, 4f);

        SpawnPlatform(width, spawnPos);
    }

    private void SpawnPlatform(float width, Vector3? position = null)
    {
        Vector3 pos = position ?? _lastPlatformEndPosition + new Vector3(width/2, 0, 0);
        
        GameObject newPlat = Instantiate(platformPrefab, pos, Quaternion.identity);
        newPlat.transform.localScale = new Vector3(width, 1, 1);

        _lastPlatformEndPosition = new Vector3(pos.x + (width/2), pos.y, 0);
    }
}