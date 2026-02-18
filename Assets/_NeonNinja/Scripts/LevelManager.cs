using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public float initialCameraSpeed = 3f;
    public float maxCameraSpeed = 15f; // Limite massimo di velocità
    public float speedIncreaseRate = 0.1f;
    
    private float currentCameraSpeed;

    [Header("Generation Settings")]
    public GameObject platformPrefab;
    public Transform generationPoint; // Punto davanti alla camera dove generare (figlio della Camera)
    public float minGap = 2f;
    public float maxGap = 5f;
    
    // Distanza dietro la camera a cui la piattaforma viene riciclata
    public float destroyDistance = 15f; 

    private Vector3 _lastPlatformEndPosition;
    
    // --- OBJECT POOLING ---
    private Queue<GameObject> _platformPool = new Queue<GameObject>();
    private Queue<GameObject> _activePlatforms = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Pre-carichiamo un po' di piattaforme nella piscina all'avvio (opzionale ma consigliato)
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(platformPrefab);
            obj.SetActive(false);
            _platformPool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Da chiamare nel GameManager dentro StartGame()
    /// </summary>
    public void StartLevelGeneration()
    {
        currentCameraSpeed = initialCameraSpeed;
        _lastPlatformEndPosition = new Vector3(-5, -2, 0); // Posizione di partenza
        SpawnPlatform(10f); // Piattaforma iniziale lunga e sicura
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // Muovi la camera
        cameraTransform.Translate(Vector3.right * currentCameraSpeed * Time.deltaTime);
        
        // Aumenta difficoltà fino al limite massimo
        if (currentCameraSpeed < maxCameraSpeed)
        {
            currentCameraSpeed += speedIncreaseRate * Time.deltaTime;
        }

        // Genera nuove piattaforme se ci stiamo avvicinando alla fine dell'ultima
        if (Vector3.Distance(cameraTransform.position, _lastPlatformEndPosition) < 20f)
        {
            GenerateNextPlatform();
        }

        // Ricicla le vecchie piattaforme
        RecyclePlatforms();
    }

    private void GenerateNextPlatform()
    {
        float gap = Random.Range(minGap, maxGap);
        float heightChange = Random.Range(-1.5f, 1.5f);
        float width = Random.Range(2f, 6f);

        Vector3 spawnPos = _lastPlatformEndPosition + new Vector3(gap + (width / 2), heightChange, 0);
        
        // Mantieni la Y entro limiti giocabili
        spawnPos.y = Mathf.Clamp(spawnPos.y, -4f, 4f);

        SpawnPlatform(width, spawnPos);
    }

    private void SpawnPlatform(float width, Vector3? position = null)
    {
        Vector3 pos = position ?? _lastPlatformEndPosition + new Vector3(width / 2, 0, 0);
        
        GameObject newPlat;

        // Se ci sono piattaforme disponibili nella pool, prendiamone una
        if (_platformPool.Count > 0)
        {
            newPlat = _platformPool.Dequeue();
            newPlat.SetActive(true);
            newPlat.transform.position = pos;
        }
        else
        {
            // Altrimenti, ne creiamo una nuova (succederà solo all'inizio se la pool si svuota)
            newPlat = Instantiate(platformPrefab, pos, Quaternion.identity);
        }

        newPlat.transform.localScale = new Vector3(width, 1, 1);
        
        // Aggiungiamo la piattaforma alla lista di quelle attive
        _activePlatforms.Enqueue(newPlat);

        _lastPlatformEndPosition = new Vector3(pos.x + (width / 2), pos.y, 0);
    }

    /// <summary>
    /// Controlla la piattaforma più vecchia. Se è fuori dallo schermo, la disattiva e la rimette in pool.
    /// </summary>
    private void RecyclePlatforms()
    {
        if (_activePlatforms.Count > 0)
        {
            // Peek guarda il primo elemento senza toglierlo
            GameObject oldestPlatform = _activePlatforms.Peek(); 

            // Se la piattaforma è molto indietro rispetto alla telecamera
            if (oldestPlatform.transform.position.x < cameraTransform.position.x - destroyDistance)
            {
                // La togliamo dalla coda delle attive
                _activePlatforms.Dequeue();
                
                // La disattiviamo e la rimettiamo in piscina
                oldestPlatform.SetActive(false);
                _platformPool.Enqueue(oldestPlatform);
            }
        }
    }
}