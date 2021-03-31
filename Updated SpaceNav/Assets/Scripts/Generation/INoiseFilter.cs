using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Noise Interface
// All scripts in this folder were coded with the following tutorial series by Sebastian Lague: https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8
public interface INoiseFilter
{
    // Method for a class in order to qualify as a noise filter.
    float Evaluate(Vector3 point);
}
