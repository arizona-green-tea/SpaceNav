using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for keeping track of all noise settings.
[System.Serializable]
public class NoiseSettings
{
    // Enumerator for different noise types.
    public enum FilterType {Simple, Rigid}
    public FilterType filterType;

    // Hide the noise settings that are not selected.
    [ConditionalHide("filterType", 0)] // Hide if enum index 0 is selected.
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)] // Hide if enum index 1 is selected.
    public RigidNoiseSettings rigidNoiseSettings;


    [System.Serializable]
    public class SimpleNoiseSettings 
    {
        // Different attributes of noise.
        public float strength = 1;
        [Range(1,8)]
        public int numLayers = 1;
        public float baseRoughness = 1;
        public float roughness = 2;
        public float persistence = 0.5f;
        public Vector3 center;
        public float minValue;
    }

    // Implements SimpleNoiseSettings since they have the same fields.
    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultipler = 0.8f;
    }
}
