using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// More rich-like terrain by applying some additional math to the original SimpleNoiseFilter.
// Most of the code is similar to SimpleNoiseFilter.
public class RigidNoiseFilter : INoiseFilter
{
    NoiseSettings.RigidNoiseSettings settings;
    Noise noise = new Noise();

    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1; // Value for weighting the noise in each layer based on the noise that came before it, allows ridges to be more detailed.

        for (int i =0; i < settings.numLayers; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.center)); // Get the absolute value to get sharper peaks and invert it by subtracting it from 1.
            v *= v; // Make ridges more pronounced by squaring the value.
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultipler);

            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = noiseValue - settings.minValue;
        return noiseValue * settings.strength;
    }
}
