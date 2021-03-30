using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script responsible for the object being attracted.
 * 1/31/2021
 */

[RequireComponent (typeof (Rigidbody))] // A rigidbody is automatically added when this script is assigned to an object in the editor.
public class GravityBody : MonoBehaviour
{
    // Reference to gravity attractor script.
    //GravityAttractor planet;
    public CelestialBody referenceBody { get; set; }
    Rigidbody rb;

    // On Game Start
    void Awake()
    {
        // Locate planet using its tag in the Unity editor.
        //planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();

        // Disable Unity's default gravity.
        GetComponent<Rigidbody>().useGravity = false;

        // Disable default rotation.
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        // Initialize rigidbody
        rb = GetComponent<Rigidbody>();
    }

    // Update() except it is called multiple times per frame (used for physics calculations)
    void FixedUpdate()
    {
        UpdateGravity();
        //rb.MovePosition(rb.position * smoothVelocity * Time.fixedDeltaTime);
        //planet.attract(transform);

    }

    private void UpdateGravity()
    {
        // Get array of all existing planets
        CelestialBody[] bodies = NBodySimulation.bodies;
        // Set initial strongest gravity to 0
        Vector3 strongestGravitationalPull = Vector3.zero;
        // Calculate the strongest gravitaitonal pull relative to the player
        foreach (CelestialBody body in bodies)
        {
            // Find the distance 
            float sqrDst = (body.Position - rb.position).sqrMagnitude;

            // Get the direction the planet is moving
            Vector3 forceDir = (body.Position - rb.position).normalized;

            // Get the acceleration of the current planet by rearranging the equation F = ma, which gives us a = F/m. We can use the equation F = G(m1m2/r^2) to determine force.
            Vector3 acceleration = forceDir * UniverseConstants.gravitationalConstant * body.mass / sqrDst;

            // Accelerate the player so that it maintains the same acceleration as the current planet
            rb.AddForce(acceleration, ForceMode.Acceleration);

            // Determine which planet has the greatest gravitational pull
            if (acceleration.sqrMagnitude > strongestGravitationalPull.sqrMagnitude)
            {
                strongestGravitationalPull = acceleration;
                referenceBody = body;
            }
        }

        // Add a downward force the the player.
        Vector3 gravityUp = -strongestGravitationalPull.normalized;
        rb.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * rb.rotation;
    }
}
