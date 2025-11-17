using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ScriptableObject qui définit un pattern d'obstacles pour un chunk
/// </summary>
[CreateAssetMenu(fileName = "New Obstacle Pattern", menuName = "Game/Obstacle Pattern")]
public class ObstaclePattern : ScriptableObject
{
    [System.Serializable]
    public class ObstaclePlacement
    {
        public GameObject obstaclePrefab;
        [Range(0f, 1f)] public float xPosition; // Position relative dans le chunk (0 = début, 1 = fin)
        public float yOffset; // Décalage vertical depuis le sol
    }
    
    [Header("Pattern Settings")]
    public string patternName;
    public List<ObstaclePlacement> obstacles = new List<ObstaclePlacement>();
    
    [Header("Difficulty")]
    [Range(0, 10)] public int difficultyLevel = 1;
    
    /// <summary>
    /// Applique ce pattern à un chunk
    /// </summary>
    public void ApplyToChunk(MapChunk chunk, float chunkWidth)
    {
        chunk.ClearObstacles();
        
        foreach (var placement in obstacles)
        {
            if (placement.obstaclePrefab != null)
            {
                float xPos = placement.xPosition * chunkWidth;
                Vector2 position = new Vector2(xPos, placement.yOffset);
                chunk.AddObstacle(placement.obstaclePrefab, position);
            }
        }
    }
}
