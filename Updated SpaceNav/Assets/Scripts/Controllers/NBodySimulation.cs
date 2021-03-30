using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simulation Controller, handles the movement of all bodies in the scene.
// Uses Sebastian Lague's Coding Adventure as a base
public class NBodySimulation : MonoBehaviour
{
    // Array of all existing celestial bodies in the scene
    public static CelestialBody[] bodies { get; private set; }


    // Initialize bodies array and time meter
    void Awake() 
    {
        bodies = FindObjectsOfType<CelestialBody>();
        Time.fixedDeltaTime = UniverseConstants.physicsTimeStep;
    }

    // Loop through entire bodies array and update all positions and velocities
    void FixedUpdate() 
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            Vector3 acceleration = CalculateAcceleration (bodies[i].Position, bodies[i]);
            bodies[i].UpdateVelocity(acceleration, UniverseConstants.physicsTimeStep);
        }

        for (int i = 0; i < bodies.Length; i++) 
        {
            bodies[i].UpdatePosition (UniverseConstants.physicsTimeStep);
        }
    }

    // Calculate the acceleration of the current celestial body
    public static Vector3 CalculateAcceleration (Vector3 point, CelestialBody ignoreBody = null) 
    {
        Vector3 acceleration = Vector3.zero;
        foreach (var body in bodies) 
        {
            if (body != ignoreBody) 
            {
                float sqrDst = (body.Position - point).sqrMagnitude;
                Vector3 forceDir = (body.Position - point).normalized;
                acceleration += forceDir * UniverseConstants.gravitationalConstant * body.mass / sqrDst;
            }
        }

        return acceleration;
    }
}
