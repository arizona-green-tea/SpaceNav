using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple noise processing that creates terrain.
public class SimpleNoiseFilter : INoiseFilter
{
    NoiseSettings.SimpleNoiseSettings settings;

    // Noise object so we can process the simplex noise algorithm.
    Noise noise = new Noise();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        // Create increasingly detailed layers (frequency increases with each layer)
        float noiseValue = 0;
        float frequency = settings.baseRoughness; 
        float amplitude = 1;

        // Loop over all the layers of the planet.
        for (int i =0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.center);
            noiseValue += (v + 1) * 0.5f * amplitude; // Maintain range from 0-1.
            frequency *= settings.roughness; // How fast frequency (amount of detail) increases. When roughness > 1, frequency increases with each layer.
            amplitude *= settings.persistence; // How fast the amplitude (height, kinda) decreases. When persistence < 1, amplitude decreases with each layer.
        }

        noiseValue = noiseValue - settings.minValue; // Allows to make the terrain recede into the planet's sphere.
        return noiseValue * settings.strength;
    }
}
