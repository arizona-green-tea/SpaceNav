using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates a noise filter based on the selected type.
public static class NoiseFilterFactory
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        // switch case for each filter type, checks what is specified in the settings.
        switch(settings.filterType)
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings.simpleNoiseSettings);
            case NoiseSettings.FilterType.Rigid:
                return new RigidNoiseFilter(settings.rigidNoiseSettings);
        }
        return null;
    }
}
