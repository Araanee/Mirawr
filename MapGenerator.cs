using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Générateur procédural de map pour un joueur (top ou bottom)
/// </summary>
public class MapGenerator : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private float chunkWidth = 20f;
    [SerializeField] private int chunksAhead = 3; // Nombre de chunks à générer en avance

    [Header("Obstacle Patterns")]
    [SerializeField] private List<ObstaclePattern> availablePatterns = new List<ObstaclePattern>();
    [SerializeField] private int currentDifficulty = 1;
    
    [Header("Generation Settings")]
    [SerializeField] private float scrollSpeed = 5f;
    [SerializeField] private bool scrollRightToLeft = true; // true = droite vers gauche, false = gauche vers droite

    private Queue<MapChunk> activeChunks = new Queue<MapChunk>();
    private float lastChunkEndPosition = 0f;
    private Transform chunksParent;
    
    // Variables pour la gestion de l'écran
    private Camera mainCamera;
    private float screenRightEdge;
    private float screenLeftEdge;

    public float ScrollSpeed
    {
        get => scrollSpeed;
        set => scrollSpeed = value;
    }

    public bool ScrollRightToLeft => scrollRightToLeft;

    private void Awake()
    {
        // Créer un parent pour tous les chunks
        GameObject parent = new GameObject("Chunks");
        parent.transform.SetParent(transform);
        parent.transform.localPosition = Vector3.zero;
        chunksParent = parent.transform;
        
        Debug.Log($"[MapGenerator] Awake - GameObject: {gameObject.name}, Active: {gameObject.activeSelf}, Enabled: {enabled}");
        Debug.Log($"[MapGenerator] ChunkPrefab: {(chunkPrefab != null ? chunkPrefab.name : "NULL")}");
    }

    private void Start()
    {
        Debug.Log($"[MapGenerator] Start - scrollRightToLeft: {scrollRightToLeft}, scrollSpeed: {scrollSpeed}");
        
        // Obtenir la caméra principale
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
        
        if (mainCamera != null)
        {
            // Calculer les bords de l'écran en coordonnées monde
            float screenHeight = 2f * mainCamera.orthographicSize;
            float screenWidth = screenHeight * mainCamera.aspect;
            screenRightEdge = mainCamera.transform.position.x + (screenWidth / 2f);
            screenLeftEdge = mainCamera.transform.position.x - (screenWidth / 2f);
        }
        else
        {
            // Valeurs par défaut si pas de caméra
            screenRightEdge = 10f;
            screenLeftEdge = -10f;
        }
        
        // Calculer la zone à couvrir : tout l'axe X visible + un peu au-delà pour sécurité
        // On veut couvrir au moins 2 écrans de largeur de chaque côté du centre de l'écran
        float screenCenterX = mainCamera != null ? mainCamera.transform.position.x : 0f;
        float coverageWidth = (screenRightEdge - screenLeftEdge) * 2f; // 2 écrans de largeur
        float startX = screenCenterX - (coverageWidth / 2f); // Commencer bien à gauche
        float endX = screenCenterX + (coverageWidth / 2f); // Finir bien à droite
        
        // Initialiser la position de départ selon le sens de défilement
        if (scrollRightToLeft)
        {
            // Pour défilement droite->gauche : commencer à gauche (startX) et aller vers la droite
            // On aligne sur le bord gauche du premier chunk
            lastChunkEndPosition = startX;
        }
        else
        {
            // Pour défilement gauche->droite : commencer à gauche (startX) et aller vers la droite
            // On aligne sur le bord gauche du premier chunk
            lastChunkEndPosition = endX;
        }
        
        // Calculer combien de chunks sont nécessaires pour couvrir toute la zone
        int totalChunks = Mathf.CeilToInt((endX - startX) / chunkWidth) + 1; // +1 pour sécurité
        
        // Générer tous les chunks initiaux pour couvrir toute la zone
        for (int i = 0; i < totalChunks; i++)
        {
            GenerateNextChunk();
        }
        
        Debug.Log($"[MapGenerator] {activeChunks.Count} chunks générés initialement sur tout l'axe X, couvrant de {startX} à {endX}, écran: {screenLeftEdge} à {screenRightEdge}");
    }

    private void Update()
    {
        // Mettre à jour les bords de l'écran (au cas où la caméra bouge)
        if (mainCamera != null)
        {
            float screenHeight = 2f * mainCamera.orthographicSize;
            float screenWidth = screenHeight * mainCamera.aspect;
            screenRightEdge = mainCamera.transform.position.x + (screenWidth / 2f);
            screenLeftEdge = mainCamera.transform.position.x - (screenWidth / 2f);
        }
        
        // Déplacer tous les chunks automatiquement
        float moveAmount = scrollSpeed * Time.deltaTime;
        if (!scrollRightToLeft)
            moveAmount = -moveAmount; // Inverser pour défilement gauche->droite

        foreach (MapChunk chunk in activeChunks)
        {
            chunk.transform.position += Vector3.left * moveAmount;
        }

        // Vérifier si des chunks sont sortis de l'écran et les recycler
        while (activeChunks.Count > 0)
        {
            MapChunk firstChunk = activeChunks.Peek();
            float chunkLeftEdge = firstChunk.transform.position.x;
            float chunkRightEdge = chunkLeftEdge + chunkWidth;
            
            bool isOutOfScreen = false;
            
            if (scrollRightToLeft)
            {
                // Chunk sorti à gauche de l'écran
                if (chunkRightEdge < screenLeftEdge)
                {
                    isOutOfScreen = true;
                }
            }
            else
            {
                // Chunk sorti à droite de l'écran
                if (chunkLeftEdge > screenRightEdge)
                {
                    isOutOfScreen = true;
                }
            }
            
            if (isOutOfScreen)
            {
                RecycleChunk();
            }
            else
            {
                break; // Les autres chunks sont encore visibles
            }
        }
        
        // Générer de nouveaux chunks si nécessaire pour maintenir le flux
        EnsureEnoughChunks();
    }

    /// <summary>
    /// Génère un nouveau chunk avec un pattern aléatoire
    /// </summary>
    private void GenerateNextChunk()
    {
        MapChunk chunk;

        if (chunkPrefab != null)
        {
            GameObject chunkObj = Instantiate(chunkPrefab, chunksParent);
            chunk = chunkObj.GetComponent<MapChunk>();
            if (chunk == null)
                chunk = chunkObj.AddComponent<MapChunk>();
        }
        else
        {
            // Créer un chunk basique si pas de prefab
            GameObject chunkObj = new GameObject("Chunk");
            chunkObj.transform.SetParent(chunksParent);
            chunk = chunkObj.AddComponent<MapChunk>();
        }

        // Positionner le chunk au bord d'entrée de l'écran
        float chunkX;
        float chunkY = transform.position.y;
        
        // Pour les deux types de défilement, on génère les chunks de gauche à droite
        // La différence est uniquement dans le sens de défilement dans Update()
        chunkX = lastChunkEndPosition;
        lastChunkEndPosition += chunkWidth; // Prochain chunk après celui-ci (vers la droite)

        chunk.transform.position = new Vector3(chunkX, chunkY, 0f);

        // Appliquer un pattern d'obstacles
        ApplyRandomPattern(chunk);

        activeChunks.Enqueue(chunk);

        if (!scrollRightToLeft)
        {
            Console.Log("Je suis pas folle");
        }
        Console.Log($"[MapGenerator] Chunk généré à position {chunk.transform.position}, total: {activeChunks.Count}");
    }

    /// <summary>
    /// Applique un pattern aléatoire au chunk
    /// </summary>
    private void ApplyRandomPattern(MapChunk chunk)
    {
        if (availablePatterns.Count == 0)
            return;

        // Filtrer les patterns selon la difficulté actuelle
        List<ObstaclePattern> validPatterns = availablePatterns.FindAll(p => p.difficultyLevel <= currentDifficulty);

        if (validPatterns.Count == 0)
            validPatterns = availablePatterns;

        ObstaclePattern selectedPattern = validPatterns[Random.Range(0, validPatterns.Count)];
        selectedPattern.ApplyToChunk(chunk, chunkWidth);
    }

    /// <summary>
    /// Recycle le chunk le plus ancien en le repositionnant au bord d'entrée
    /// </summary>
    private void RecycleChunk()
    {
        MapChunk oldChunk = activeChunks.Dequeue();

        // Repositionner le chunk au bord d'entrée de l'écran
        float chunkX;
        float chunkY = transform.position.y;
        
        // Pour les deux types de défilement, on repositionne les chunks de gauche à droite
        // La différence est uniquement dans le sens de défilement dans Update()
        if (scrollRightToLeft)
        {
            // Repositionner au bord DROIT de l'écran (pour défilement droite->gauche)
            chunkX = lastChunkEndPosition;
            lastChunkEndPosition += chunkWidth;
        }
        else
        {
            // Repositionner au bord GAUCHE de l'écran (pour défilement gauche->droite)
            chunkX = lastChunkEndPosition;
            lastChunkEndPosition += chunkWidth;
        }

        oldChunk.transform.position = new Vector3(chunkX, chunkY, 0f);

        // Appliquer un nouveau pattern
        ApplyRandomPattern(oldChunk);

        activeChunks.Enqueue(oldChunk);
    }
    
    /// <summary>
    /// S'assure qu'il y a assez de chunks visibles à l'écran
    /// </summary>
    private void EnsureEnoughChunks()
    {
        if (activeChunks.Count == 0)
        {
            GenerateNextChunk();
            return;
        }
        
        // Vérifier le dernier chunk (le plus récent)
        MapChunk lastChunk = null;
        foreach (MapChunk chunk in activeChunks)
        {
            lastChunk = chunk;
        }
        
        if (lastChunk != null)
        {
            float lastChunkRightEdge = lastChunk.transform.position.x + chunkWidth;
            float lastChunkLeftEdge = lastChunk.transform.position.x;
            
            bool needMoreChunks = false;
            
            if (scrollRightToLeft)
            {
                // Vérifier si le dernier chunk atteint encore le bord droit de l'écran
                if (lastChunkRightEdge < screenRightEdge + chunkWidth)
                {
                    needMoreChunks = true;
                }
            }
            else
            {
                // Vérifier si le dernier chunk atteint encore le bord gauche de l'écran
                if (lastChunkLeftEdge > screenLeftEdge - chunkWidth)
                {
                    needMoreChunks = true;
                }
            }
            
            if (needMoreChunks)
            {
                GenerateNextChunk();
            }
        }
    }

    /// <summary>
    /// Augmente la difficulté progressivement
    /// </summary>
    public void IncreaseDifficulty()
    {
        currentDifficulty = Mathf.Min(currentDifficulty + 1, 10);
    }

    /// <summary>
    /// Réinitialise le générateur
    /// </summary>
    public void Reset()
    {
        // Détruire tous les chunks existants
        while (activeChunks.Count > 0)
        {
            MapChunk chunk = activeChunks.Dequeue();
            if (chunk != null)
                Destroy(chunk.gameObject);
        }

        // Réinitialiser
        lastChunkEndPosition = 0f;
        currentDifficulty = 1;

        // Régénérer les chunks initiaux
        for (int i = 0; i < chunksAhead; i++)
        {
            GenerateNextChunk();
        }
    }
}

