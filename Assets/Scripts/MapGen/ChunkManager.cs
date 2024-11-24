using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChunkManager : MonoBehaviour
{
    [Header("Chunk Settings")]
    public int chunkSize = 128; // Size of each chunk (e.g., 16x16 tiles)
    public int scale = 100; // Scale factor applied to chunks and meshes
    public int viewDistance = 1; // View distance in chunks (this will be in terms of scaled chunks)

    // Serialized fields for linking to the scene objects
    [SerializeField] private Transform playerPos; // Player position to track chunk loading/unloading
    [SerializeField] private Material chunkMaterial; // Material to apply to chunks

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    private Vector2Int currentPlayerChunk; // Track the player's current chunk

    private MapGen mapGen; // Reference to the MapGen script

    // Start is called once when the script is initialized
    void Start()
    {
        mapGen = GetComponent<MapGen>(); // Cache the MapGen reference

        // Initialize the player's current chunk based on the scaled chunk size
        currentPlayerChunk = new Vector2Int(
            Mathf.FloorToInt(playerPos.position.x / (chunkSize * scale)),
            Mathf.FloorToInt(playerPos.position.z / (chunkSize * scale))
        );

        // Load the initial set of chunks around the player
        UpdateChunks(playerPos.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the player's chunk based on their current position
        Vector2Int newPlayerChunk = new Vector2Int(
            Mathf.FloorToInt(playerPos.position.x / (chunkSize * scale)),
            Mathf.FloorToInt(playerPos.position.z / (chunkSize * scale))
        );

        // Only update chunks if the player has moved to a new chunk
        if (newPlayerChunk != currentPlayerChunk)
        {
            currentPlayerChunk = newPlayerChunk;
            UpdateChunks(playerPos.position);
        }
    }

    // Updates chunk loading and unloading based on player position
    void UpdateChunks(Vector3 playerPosition)
    {
        // Calculate the player's chunk coordinate based on scaled chunk size
        Vector2Int currentChunkCoord = new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / (chunkSize * scale)),
            Mathf.FloorToInt(playerPosition.z / (chunkSize * scale))
        );

        // Debugging: print current chunk coordinates and player position
        // Debug.Log($"Player Position: {playerPosition}, Current Chunk: {currentChunkCoord}");

        // Load nearby chunks within the scaled view distance
        for (int y = -viewDistance; y <= viewDistance; y++)
        {
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                Vector2Int chunkCoord = currentChunkCoord + new Vector2Int(x, y);
                Debug.Log(chunkCoord);

                // Check if the chunk is already loaded
                if (!loadedChunks.ContainsKey(chunkCoord))
                {
                    CreateChunk(chunkCoord); // Create a new chunk if it doesn't exist
                    Debug.Log($"Creating Chunk: {chunkCoord}");
                }
            }
        }

        // Unload distant chunks that are beyond the scaled view distance
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();

        foreach (var chunkCoord in loadedChunks.Keys.ToList())
        {
            // Use the scaled distance check to compare chunk distances
            float scaledDistance = Vector2Int.Distance(chunkCoord, currentChunkCoord);
            // Debug.Log(scaledDistance);
            if (scaledDistance > viewDistance)
            {
                chunksToUnload.Add(chunkCoord);
                Debug.Log($"Chunk {chunkCoord} is too far, adding to unload list");
            }
        }

        // Unload chunks that are too far away
        foreach (var chunkCoord in chunksToUnload)
        {
            UnloadChunk(chunkCoord);
        }
    }

    void OnDrawGizmos()
    {
        if (loadedChunks == null) return;

        Gizmos.color = Color.green;
        foreach (var chunk in loadedChunks.Values)
        {
            MeshFilter meshFilter = chunk.gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.mesh == null) continue;

            Vector3[] vertices = meshFilter.mesh.vertices;
            foreach (var vertex in vertices)
            {
                Vector3 worldVertex = chunk.gameObject.transform.TransformPoint(vertex);
                Gizmos.DrawSphere(worldVertex, 0.1f); // Draw a small sphere at each vertex
            }
        }
    }

    void CreateChunk(Vector2Int chunkCoord)
    {
        // Create a new GameObject for the chunk
        GameObject chunkObject = new GameObject($"Chunk {chunkCoord.x}, {chunkCoord.y}");

        // Adjust the chunk's position based on chunk size and scale factor
        chunkObject.transform.position = new Vector3(
            chunkCoord.x * chunkSize * scale, 
            0, 
            chunkCoord.y * chunkSize * scale
        );
        chunkObject.transform.localScale = Vector3.one; // No scaling on the GameObject itself

        // Add the Chunk component
        Chunk chunk = chunkObject.AddComponent<Chunk>();

        // Generate the noise map for the chunk
        Vector2 offset = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);
        float[,] noiseMap = mapGen.GenerateNoiseMap(chunkSize + 1, chunkSize + 1, scale * 10, offset, 4, 0.5f, 2f);

        // Initialize the chunk with its position and noise map
        chunk.Initialize(chunkCoord, noiseMap);

        // Generate the mesh for the chunk
        MeshFilter meshFilter = chunkObject.AddComponent<MeshFilter>();
        Mesh mesh = mapGen.GenerateTerrainMesh(noiseMap, chunkSize);

        // Scale the mesh based on the scale factor
        mesh = mapGen.ScaleMesh(mesh, scale); // Scale mesh vertices as needed
        meshFilter.mesh = mesh;

        // Add a MeshRenderer to render the mesh
        MeshRenderer meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(chunkMaterial); // Create a new material instance
        meshRenderer.material.mainTexture = chunk.GetNoiseMapTexture(); // Apply the texture

        // Add a MeshCollider for collision detection
        MeshCollider meshCollider = chunkObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        // Add the chunk to the loadedChunks dictionary
        loadedChunks.Add(chunkCoord, chunk);

        // Debugging: print chunk creation
        Debug.Log($"Created Chunk at {chunkCoord}");
    }

    void UnloadChunk(Vector2Int chunkCoord)
    {
        if (loadedChunks.TryGetValue(chunkCoord, out Chunk chunk))
        {
            Destroy(chunk.gameObject); // Destroy the chunk's GameObject
            loadedChunks.Remove(chunkCoord); // Remove it from the loadedChunks dictionary
            // Debug.Log($"Chunk {chunkCoord} unloaded.");
        }
    }
}
