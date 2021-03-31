using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherits from scriptable object so we can create assets from this script.
// Holds the physical properties of the planet (size, level of detail through noise).
// All scripts in this folder were coded with the following tutorial series by Sebastian Lague: https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    public float planetRadius = 1;
    public NoiseLayer[] noiseLayers;

    // For separate noise layers.
    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask; // Allows noise on the current layer to only be generated on the first layer of noise.
        public NoiseSettings noiseSettings;
    }
}
