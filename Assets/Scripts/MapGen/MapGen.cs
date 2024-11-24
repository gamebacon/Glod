using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    [SerializeField]
    private GameObject test;

    void Start() {
        MeshFilter meshFilter = test.GetComponent<MeshFilter>();
        float[,] noiseMap = GenerateNoiseMap(20, 20, 10, new Vector2(1, 1));
        meshFilter.mesh = GenerateTerrainMesh(noiseMap, 10);
    }

    public float[,] GenerateNoiseMap(int width, int height, float scale, Vector2 offset, int octaves = 4, float persistence = 0.5f, float lacunarity = 2f)
    {
        float[,] noiseMap = new float[width, height];
        float amplitude = 1;
        float frequency = 1;
        float maxPossibleHeight = 1;

        for (int i = 0; i < octaves; i++) {
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    // Adjust noise sampling by offset
                    float sampleX = (x + offset.x) / scale * frequency;
                    float sampleY = (y + offset.y) / scale * frequency;
                    noiseMap[x, y] += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;
                }
            }
            maxPossibleHeight += amplitude;
            amplitude *= persistence;
            frequency *= lacunarity;
        }

        // Normalize height values
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                noiseMap[x, y] /= maxPossibleHeight;
            }
        }
        return noiseMap;
    }

public Mesh GenerateTerrainMesh(float[,] noiseMap, int chunkSize)
{
    Mesh terrainMesh = new Mesh();
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    int width = noiseMap.GetLength(0);
    int height = noiseMap.GetLength(1);
    int heightMultiplier = 4;

    for (int y = 0; y < height; y++) // We use height (chunkSize + 1) for loop bounds
    {
        for (int x = 0; x < width; x++) // Likewise for width
        {
            // Get the noise value for the current position
            float terrainHeight = noiseMap[x, y];

            // Scale the height if needed
            float scaledHeight = terrainHeight * heightMultiplier; // Example scaling factor for height

            // Add the vertex at the current position
            Vector3 vert = new Vector3(x, scaledHeight, y);
            vertices.Add(vert);
            uvs.Add(new Vector2((float)x / chunkSize, (float)y / chunkSize));

            // Add triangles to form the mesh
            if (x < chunkSize && y < chunkSize)
            {
                int i = x + y * (chunkSize + 1);
                triangles.Add(i);
                triangles.Add(i + chunkSize + 1);
                triangles.Add(i + 1);

                triangles.Add(i + 1);
                triangles.Add(i + chunkSize + 1);
                triangles.Add(i + chunkSize + 2);
            }
        }
    }

    terrainMesh.vertices = vertices.ToArray();
    terrainMesh.triangles = triangles.ToArray();
    terrainMesh.uv = uvs.ToArray();
    terrainMesh.RecalculateNormals(); // Recalculate normals to ensure proper lighting

    return terrainMesh;
}

public Mesh ScaleMesh(Mesh mesh, float scaleFactor)
{
    Vector3[] vertices = mesh.vertices;
    for (int i = 0; i < vertices.Length; i++)
    {
        vertices[i] *= scaleFactor; // Scale each vertex
    }

    mesh.vertices = vertices;
    mesh.RecalculateBounds(); // Recalculate the mesh bounds after scaling
    return mesh;
}

}
