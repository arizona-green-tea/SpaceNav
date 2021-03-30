using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that holds universal constants.
public class UniverseConstants
{
    /* Gravitational Constant, used in equation F = G(m1m2/r^2)
     * G = 6.7 * 10^-11 in the real world, but it can be tuned in
     * this simulation accordingly.
     */
    public const float gravitationalConstant = 0.0001f;

    /* Value passed into positional and vector updates so that
     * all bodies adhere to the same time meter.
     * Controls time scale, test accordingly.
     */
    public const float physicsTimeStep = 0.01f;
    // Anything greater than 1f results in some wonky interactions.
    // Make sure celestial bodies don't move so fast they escape the gravitatational pull of interacting entities.
}
