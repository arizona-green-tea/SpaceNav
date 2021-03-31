using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates the color based on what is supplied in the settings, apply shaders based on elevation.
// All scripts in this folder were coded with the following tutorial series by Sebastian Lague: https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public class ColorGenerator
{
    // Color settings.
    ColorSettings settings;

    // Texture fields.
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoisefilter;

    // Initialize settings and apply texture if none is selected.
    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if(texture == null || texture.height != settings.biomeColorSettings.biomes.Length)
        {
            // Height corresponds to num of biomes.
            // Store biome and ocean textures.
            texture = new Texture2D(textureResolution * 2, settings.biomeColorSettings.biomes.Length, TextureFormat.RGBA32, false);
        }
        // Initialize the noise settings of the biome.
        biomeNoisefilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    // Update the colors based on the elevation.
    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    // Returns 0 for first biome and 1 for last biome. Gauges how far the point is across the planet relative to the south pole.
    // Allows us to calculate what biome the current point is in.
    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoisefilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
        float biomeIndex = 0;
        int numBiomes = settings.biomeColorSettings.biomes.Length;
        float blendRange = settings.biomeColorSettings.blendAmount / 2f + 0.001f;
        
        for (int i = 0; i < numBiomes; i++)
        {
            // Calculate distance of biome's startHeight from the current height percent.
            float dst = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            // Depends on whether above distance is in the range of the blending distance/range.
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

        // Ensures that biome index is divided by 0 or a negative number.
        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    // Apply colors over all given points using each biome settings.
    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];
        int colorIndex = 0;
        foreach (var biome in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color gradientCol;
                // If the color index is in the first half of the texture resolution, apply ocean settings. Else, apply biome settings.
                if (i < textureResolution)
                {
                    gradientCol = settings.oceanColor.Evaluate(i / (textureResolution - 1f));
                }
                else
                {
                    gradientCol = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                }
                Color tintCol = biome.tint;
                // Apply tint to the biome. If there is no tint, then it is multiplied by a tintPercent of 0.
                colors[colorIndex] = gradientCol * (1 - biome.tintPercent) + tintCol * biome.tintPercent;
                colorIndex++;
            }
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.planetMaterial.SetTexture("_texture", texture);
    }
}
