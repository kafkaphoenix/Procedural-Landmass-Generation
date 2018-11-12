using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData {

    public Noise.NormalizeMode normalizeMode;

    public float noiseScale;

    public int octaves; // Layers of noise
    [Range(0, 1)]
    public float persistance; // Controls decrease in amplitude of octaves - Affect how many small features influence over the all map
    public float lacunarity; // Controls increase in frequency of octaves - A high value increase the number of small features

    public int seed;
    public Vector2 offset;

    protected override void OnValidate()
    {
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        else if (octaves < 0)
        {
            octaves = 0;
        }

        base.OnValidate();
    }
}
