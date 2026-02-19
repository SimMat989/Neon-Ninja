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
    public Transform initialGroundEnd; // AGGIUNTO: Il punto in cui finisce il tuo pavimento fisso
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
    /// 
    public void StartLevelGeneration()
    {
        currentCameraSpeed = initialCameraSpeed;
        
        // 1. Diciamo al sistema di partire da dove finisce il tuo terreno fisso
        if (initialGroundEnd != null)
        {
            // Prende la X e la Y della fine del tuo terreno
            _lastPlatformEndPosition = initialGroundEnd.position; 
        }
        else
        {
            // Fallback di sicurezza se ti dimentichi di assegnarlo
            _lastPlatformEndPosition = new Vector3(-5f, -2f, 0f);
            SpawnPlatform(10f); 
        }

        // 2. PRE-GENERAZIONE ISTANTANEA
        // Genera piattaforme procedurali dalla fine del tuo terreno fino al Generation Point
        if (generationPoint != null)
        {
            while (_lastPlatformEndPosition.x < generationPoint.position.x)
            {
                GenerateNextPlatform();
            }
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        // Muovi la telecamera (e di conseguenza anche il Generation Point che ne è figlio)
        cameraTransform.Translate(Vector3.right * currentCameraSpeed * Time.deltaTime);
        
        if (currentCameraSpeed < maxCameraSpeed)
        {
            currentCameraSpeed += speedIncreaseRate * Time.deltaTime;
        }

        // 3. IL VERO UTILIZZO DEL GENERATION POINT:
        // Appena il Generation Point (avanzando) sorpassa la fine dell'ultima piattaforma creata, ne genera un'altra.
        if (generationPoint != null && _lastPlatformEndPosition.x < generationPoint.position.x)
        {
            GenerateNextPlatform();
        }

        // Ricicla le piattaforme uscite dallo schermo
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