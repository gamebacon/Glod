using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;             // The prefab to spawn (tree, rock, stump, etc.)
    public int quantity;                  // Number of this object type to spawn
    public float minScale = 0.2f;         // Minimum scale for size variation
    public float maxScale = 1f;         // Maximum scale for size variation
    public float minSpacing = 3f;         // Minimum spacing from other objects

}

public class EnvironmentSpawner : MonoBehaviour
{
    public List<SpawnableObject> objectsToSpawn;   // List of objects to spawn (trees, rocks, etc.)
    [SerializeField] private GameObject localPlayerPrefab;   // Localplayer for singleplayer 
    private Terrain terrain;                       // Reference to the active terrain

    private List<Vector3> _spawnedPositions = new List<Vector3>();  // Track spawned positions for spacing
    private Vector3 _terrainCenter;                // Center of the terrain

    private ObjectManager _objectManager;

    private void Start()
    {
        // Retrieve the active terrain and calculate its center
        terrain = Terrain.activeTerrain;
        if (terrain == null)
        {
            Debug.LogError("No active terrain found! Please add a terrain to the scene.");
            return;
        }

        _objectManager = ObjectManager.GetInstance();


        _terrainCenter = terrain.transform.position + new Vector3(terrain.terrainData.size.x / 2, 0, terrain.terrainData.size.z / 2);

        SpawnEnvironmentObjects();

        if (GameManager.GetInstance().isSinglePlayer)
        {
            SpawnSinglePlayer();
        }
    }

    public void SpawnSinglePlayer()
    {
        Vector3 spawnPos = GetRandomPositionWithinArea();
        Instantiate(localPlayerPrefab, spawnPos, Quaternion.identity);
    }


    public void SpawnEnvironmentObjects()
    {
        int seed = GameManager.GetInstance().gameSettings.seed;
        Debug.Log(seed);
        PerlinNoise _perlinNoise = new PerlinNoise(seed);
        Queue<Vector3> poses = _perlinNoise.Generate();

        foreach (SpawnableObject obj in objectsToSpawn)
        {

            for (int i = 0; i < obj.quantity; i++)
            {
                Vector3 spawnPosition = poses.Count > 0 ? poses.Dequeue() : Vector3.zero;

                // Adjust spawn position to be on the terrain surface
                spawnPosition = GetTerrainHeightAdjustedPosition(spawnPosition);

                // Instantiate the object at the calculated position
                GameObject newObject = _objectManager.SpawnObject(obj.prefab, spawnPosition); // Instantiate(obj.prefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                
                // Randomize the scale
                float randomScale = Random.Range(obj.minScale, obj.maxScale);
                newObject.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

                // Add position to list to ensure spacing for future objects
                _spawnedPositions.Add(spawnPosition);
            }
        }
    }

    private Vector3 GetRandomPositionWithinArea()
    {
        Vector3 size = terrain.terrainData.size;

        float xPosition = Random.Range(-size.x / 2, size.x/ 2);
        float zPosition = Random.Range(-size.z / 2, size.z / 2);

        // Add the random offsets to the terrain center to create the centered spawn position
        Vector3  pos = new Vector3(xPosition, 0, zPosition) + _terrainCenter;

        return GetTerrainHeightAdjustedPosition(pos, .5f);
    }

    private Vector3 GetTerrainHeightAdjustedPosition(Vector3 position, float yOffset = 0)
    {
        if (terrain == null) return position;  // Fallback in case terrain is missing

        // Use the terrain's SampleHeight method to get the terrain height at the x, z coordinates
        float terrainHeight = terrain.SampleHeight(position);

        // Update y-position to match terrain height
        return new Vector3(position.x, terrainHeight + yOffset, position.z);
    }

    private bool IsTooCloseToOtherObjects(Vector3 position, float minSpacing)
    {
        foreach (Vector3 spawnedPosition in _spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPosition) < minSpacing)
            {
                return true;
            }
        }
        return false;
    }
}
