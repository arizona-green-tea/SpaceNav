using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Controller that is responsible for the planet's gravity.
 * Ensures that objects are pulled toward's the attractor's center.
 * 1/31/2021
 */

public class GravityAttractor : MonoBehaviour
{
    // Declare instances

    // Gravity value
    public float gravity = -10f;

    // Called by each gravity body (the object being attracted), passes in its own transform (3d object)
    public void attract(Transform body)
    {
        // Orient the body so its y axis faces the center of the planet.
        // Determine direction by subtracting the position between two objects.
        // Normalized so length remains constant.
        Vector3 targetDir = (body.position - transform.position).normalized;

        // Variable to represent the up direction of the body.
        Vector3 bodyUp = body.up; 

        // Set the body's rotation to face up.
        body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;

        // Apply a downward force to the object's rigidbody, simulating gravity.
        body.GetComponent<Rigidbody>().AddForce(targetDir * gravity);
    }
}
