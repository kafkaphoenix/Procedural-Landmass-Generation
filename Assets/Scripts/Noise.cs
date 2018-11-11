using System.Collections;
using UnityEngine;

public static class Noise
{
    // Global is estimating min and max
    public enum NormalizeMode { Local, Global};

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight,  int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPosibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPosibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2; //to center the map when changing lacunarity
        float halfHeight = mapHeight / 2;

        float sampleX, sampleY, perlinValue, noiseHeight;
        float normalizedHeight;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                amplitude = 1;
                frequency = 1;
                noiseHeight = 0;
                for (int i = 0; i < octaves; i++)
                {
                    sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1; // range -1 1
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // if we are not doing an endless map this is the better way of doing things cause 
                // we'll know what the exact min and max noise hight so we can ensure the full range
                // of values is used (local)
                if (normalizeMode == NormalizeMode.Local) {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                } else { 
                    normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPosibleHeight / 2f); // We divide maxPosibleHeight to soft it cause is too big
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight,0, int.MaxValue);
                }
                
            }
        }

        return noiseMap;
    }

}
