  
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPointManager : MonoBehaviour {

    public float distanceThreshold = 1000;
    public List<Transform> physicsObjects { get; set; }
    public ShipController ship;
    FirstPersonController player;
    public event System.Action PostFloatingOriginUpdate;
    CelestialBody[] bodies;

    // On Initialization
    // Add all objects in the simulation (bar the player) to the physicsObjects list.
    void Awake () 
    {
        bodies = FindObjectsOfType<CelestialBody>();
        physicsObjects = new List<Transform>();
        physicsObjects.Add (ship.transform);
        foreach (var c in bodies) 
        {
            physicsObjects.Add (c.transform);
        }
    }

    // Limit Framerate
    private void Start() 
    {
        Application.targetFrameRate = 60;
    }

    // Check if the player exists.
    private void Update() 
    {
        var player = FindObjectOfType<FirstPersonController>();
        if(!ship.hasPilot)
        {
            if(!physicsObjects.Contains(player.transform))
            {
                physicsObjects.Add(player.transform);
            }
        }
    }

    // If the player exists, move all objects to the simulation's center.
    void LateUpdate () 
    {
        if(!ship.hasPilot)
        {
            if(FindObjectOfType<FirstPersonController>() != null)
            {
                UpdateFloatingOrigin ();
            }
            if (PostFloatingOriginUpdate != null) 
            {
                PostFloatingOriginUpdate ();
            }
        }
    }

    // If the player's current position exceeds a specific threshold (if it gets far away enough), then all objects back to the center of the world.
    private void UpdateFloatingOrigin () 
    {
        Vector3 originOffset = FindObjectOfType<FirstPersonController>().transform.position;
        float dstFromOrigin = originOffset.magnitude;
        if (dstFromOrigin > distanceThreshold) // Make sure dst does not exceed 1
        {
            foreach (Transform t in physicsObjects) 
            {
                //t.position -= originOffset;
                if(t != null)
                {
                    t.position -= originOffset;
                    //print("moving" + t); // for debugging
                }
                
            }
        }
    }

    public void removePlayerFromList()
    {
        physicsObjects.Remove(player.transform);
    }

}