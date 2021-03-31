using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps track of the minimum and maximum elevations.
// All scripts in this folder were coded with the following tutorial series by Sebastian Lague: https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public class MinMax
{
    public float Min { get; private set; }
    public float Max { get; private set; }

    public MinMax()
    {
        // Initialize min and max default values.
        Min = float.MaxValue;
        Max = float.MinValue;
    }

    // Update the min and max values accordingly.
    public void AddValue(float v)
    {
        if (v > Max)
        {
            Max = v;
        }

        if (v < Min)
        {
            Min = v;
        }
    }

}
