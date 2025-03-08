using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise
{
    private int size = 1000;
    private int seed = 123;

    private float minSpacing = 20f;

    // private float treeSizeMultiplier = 3;
    private float treePosOffset = 5;

    private float noiseScale = 0.001f;

    private System.Random prng;

    public PerlinNoise(int seed)
    {
        this.seed = seed;
    }
    public Queue<Vector3> Generate()
    {
        Queue<Vector3> positions = new Queue<Vector3>();
        // Initialize the PRNG with the seed
        prng = new System.Random(seed);

        // Set the ground size
        // ground.transform.localScale = new Vector3(size / 10, 1, size / 10);

        float threshold = 0.5f;
        for (float x = -size / 2; x <= size / 2; x += minSpacing)
        {
            for (float z = -size / 2; z <= size / 2; z += minSpacing)
            {
                float noiseValue = Mathf.PerlinNoise((x + seed) * noiseScale, (z + seed) * noiseScale);

                if (noiseValue < threshold)
                {
                    continue;
                }

                // Use seed-based random offsets
                float offsetX = (float)(prng.NextDouble() * 2 - 1) * treePosOffset;
                float offsetZ = (float)(prng.NextDouble() * 2 - 1) * treePosOffset;

                int mapOffsetX = size / 2;
                int mapOffsetZ = size / 2;

                Vector3 position = new Vector3(x + offsetX + mapOffsetX, 0, z + offsetZ + mapOffsetZ);
                positions.Enqueue(position);

                /*
                GameObject tree = Instantiate(treePrefab, position, Quaternion.identity, parent.transform);

                // Calculate the scale based on noise value
                float scale = Mathf.Pow(Mathf.Lerp(0.5f, 2f, noiseValue), treeSizeMultiplier);
                tree.transform.localScale = new Vector3(scale, scale, scale);
                */
            }
        }
        return positions;
    }

}

