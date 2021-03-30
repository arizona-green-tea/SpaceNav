using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Inherits from scriptable object so we can create assets from this script.
// Holds the physical properties of the planet (size, level of detail through noise).
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
