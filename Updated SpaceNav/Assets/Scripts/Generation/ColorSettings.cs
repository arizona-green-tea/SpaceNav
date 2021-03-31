using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Something happened with this file, not sure why it was acting up earlier.
// Color settings for the planet.
// All scripts in this folder were coded with the following tutorial series by Sebastian Lague: https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Material planetMaterial;
    public BiomeColorSettings biomeColorSettings;
    public Gradient oceanColor;

    // Color settings for each specific biome.
    [System.Serializable]
    public class BiomeColorSettings
    {
        public Biome[] biomes;
        // Noise settings for disturbing biome boundaries.
        public NoiseSettings noise;
        // How far the noise shifts the biomes.
        public float noiseOffset;
        public float noiseStrength;
        [Range(0,1)]
        public float blendAmount;

        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            [Range(0,1)]
            public float startHeight;
            [Range(0,1)]
            public float tintPercent;
            
        }
    }
}
