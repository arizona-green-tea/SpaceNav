using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates a noise filter based on the selected type.
// All scripts in this folder were coded with the following tutorial series by Sebastian Lague: https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
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
