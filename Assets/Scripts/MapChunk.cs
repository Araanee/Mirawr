using UnityEngine;

/// <summary>
/// Représente un segment de map réutilisable avec ses obstacles
/// </summary>
public class MapChunk : MonoBehaviour
{
    [Header("Chunk Settings")]
    [SerializeField] private float chunkWidth = 20f;
    [SerializeField] private Transform obstaclesParent;
    
    public float ChunkWidth => chunkWidth;
    
    /// <summary>
    /// Nettoie tous les obstacles du chunk
    /// </summary>
    public void ClearObstacles()
    {
        if (obstaclesParent != null)
        {
            foreach (Transform child in obstaclesParent)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }
    
    /// <summary>
    /// Ajoute un obstacle au chunk
    /// </summary>
    public void AddObstacle(GameObject obstaclePrefab, Vector2 position)
    {
        if (obstaclesParent == null)
        {
            GameObject parent = new GameObject("Obstacles");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = Vector3.zero;
            obstaclesParent = parent.transform;
        }
        
        GameObject obstacle = Instantiate(obstaclePrefab, obstaclesParent);
        obstacle.transform.localPosition = position;
    }
    
    private void OnDrawGizmos()
    {
        // Visualisation du chunk dans l'éditeur
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position + Vector3.right * (chunkWidth / 2f);
        Gizmos.DrawWireCube(center, new Vector3(chunkWidth, 10f, 0f));
    }
}
