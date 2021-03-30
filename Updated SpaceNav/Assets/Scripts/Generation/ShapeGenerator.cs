using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generate the planet's physical attributes (size and terrain).
public class ShapeGenerator
{
    ShapeSettings settings;

    // Array of different noise filters.
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;

    // Update the planet according to the current shape settings.
    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings; // Assign the current shape settings. 
        noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++) // Loop through all noise filters and create a noise layer of the specified type.
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
        elevationMinMax = new MinMax();
    }

    // Calculates the current elevation of the point.
    public float CalculateUnscaledGeneration(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        // Determine points where the first layer of noise is petruding.
        if(noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if(settings.noiseLayers[0].enabled)
            {
                elevation = firstLayerValue;
            }
        }

        // Loop through all noise filters and update the elevation of the planet's points accordingly.
        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if(settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        elevationMinMax.AddValue(elevation);
        return elevation;
    }

    // Scale the elevation according to the planet's radius.
    public float GetScaledElevation(float unscaledElevation)
    {
        float elevation = Mathf.Max(0, unscaledElevation);
        elevation = settings.planetRadius * (1 + elevation);
        return elevation;
    }


}
