using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Noise Interface
public interface INoiseFilter
{
    // Method for a class in order to qualify as a noise filter.
    float Evaluate(Vector3 point);
}
