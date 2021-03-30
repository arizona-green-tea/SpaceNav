using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof (Rigidbody))] // Automatically assign Rigidbody.
// Uses Sebastian Lague's Coding Adventure as a base
public class CelestialBody : MonoBehaviour
{
    ShapeSettings settings;
    // Planet fields.
    public float radius;
    // Affects player and surrounding bodies.
    public float surfaceGravity;
    public Vector3 initialVelocity;
    public string bodyName = "placeholder"; // Can be changed in editor
    // NOTE: Implement custom name and variable to keep track of planet names
    // if using procedural generation for all planets.

    // Planet's physical qualities.
    Transform meshHolder;

    // Setters and Getters
    public Vector3 velocity { get; private set; }
    public float mass { get; private set; }

    // Physics Controller.
    Rigidbody rb;
    
    // Called once object has been initialized.
    void Awake() 
    {
        // Assign planet's rigidbody and physical values.
        rb = GetComponent<Rigidbody>();
        mass = surfaceGravity * radius * radius / UniverseConstants.gravitationalConstant;
        rb.mass = mass;
        velocity = initialVelocity;
        gameObject.name = bodyName;
    }

    // Update the velocity of all planets so they constantly orbit.
    // Newton's Law of Universal Gravitation: F = G(m1m2/r^2).
    public void UpdateVelocity(CelestialBody[] allBodies, float timeStep)
    {
        // Loop through all celestial bodies in the scene.
        foreach(var otherBody in allBodies)
        {
            if(otherBody != this)
            {
                // Subtract positions to get distance
                float sqrDst = (otherBody.rb.position - rb.position).sqrMagnitude;
                
                // Direction force will be applied (direction planet is moving)
                Vector3 forceDir = (otherBody.rb.position - rb.position).normalized;

                Vector3 acceleration = forceDir * UniverseConstants.gravitationalConstant * otherBody.mass / sqrDst;
                velocity += acceleration * timeStep;
            }
        }
    }

    // Overloaded, continually update celestial body.
    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        velocity += acceleration * timeStep;
    }

    // Moves planet according to velocity.
    public void UpdatePosition(float timeStep)
    {
        rb.MovePosition(rb.position + velocity * timeStep);
    }

    // Determine the planet's mass using the rearranged equation of F = G(m1m2/r^2)
    // Only works in editor.
    void OnValidate() 
    {
        mass = surfaceGravity * radius * radius / UniverseConstants.gravitationalConstant;
        // Grab the mesh (hitbox/appearance) of the planet
        //meshHolder = transform.GetChild(0);
        // Scale the mesh according to the given radius
        //settings.planetRadius = radius;
        // Set the name of the object
        gameObject.name = bodyName;
    }

    // Rigidbody getter
    public Rigidbody Rigidbody
    {
        get
        {
            return rb;
        }
    }

    // Position getter
    public Vector3 Position
    {
        get
        {
            return rb.position;
        }
    }

    public string getName()
    {
        return bodyName;
    }
}
