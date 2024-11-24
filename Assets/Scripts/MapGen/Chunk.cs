using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Vector2Int position; // Position of the chunk in world space

    [SerializeField]
    private float[,] noiseMap; // The noise map data for this chunk

    // Method to initialize the chunk
    public void Initialize(Vector2Int position, float[,] noiseMap)
    {
        this.position = position;
        this.noiseMap = noiseMap;
    }

    public Texture2D GetNoiseMapTexture()
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        Texture2D texture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = noiseMap[x, y];
                texture.SetPixel(x, y, new Color(value, value, value));
            }
        }

        texture.Apply();
        return texture;
    }
}
