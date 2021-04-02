using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 2/28/2020
 * Controls ship movement.
 */
public class ShipController : MonoBehaviour
{
    // Thrust Fields
    private Vector3 thrust;
    public float thrustMultiplier = 20f;

    // Rotation Fields, smooth is the interpolated version of target for best camera movement.
    private Quaternion targetRotation;
    private Quaternion smoothRotation;
    public float rotationSpeed = 5f;
    public float rollSpeed = 30f;
    public float rotSmoothSpeed = 10f;

    // Ship's own camera
    private Transform shipCamera;

    // Layer Mask for checking if the ship is grounded.
    public LayerMask groundedMask;

    // Input Fields.
    private KeyCode up = KeyCode.Space;
    private KeyCode down = KeyCode.LeftShift;
    private KeyCode left = KeyCode.A;
    private KeyCode right = KeyCode.D;
    private KeyCode forward = KeyCode.W;
    private KeyCode backward = KeyCode.S;
    private KeyCode spinCounterClockwise = KeyCode.Q;
    private KeyCode spinClockwise = KeyCode.E;

    // Speed Modifiers
    public KeyCode boost = KeyCode.Tab;
    public KeyCode slow = KeyCode.R;

    // leave ship
    private KeyCode escapeShip = KeyCode.F;
    
    // Player Pilot
    public GameObject pilot;

    // Check if player is currently in the ship.
    public bool hasPilot { get; private set; }

    // Count the amount of times the ship has touched a planet.
    int numCollisionTouches = 0;

    public bool grounded { get; private set; }
    // Rigidbody
    private Rigidbody rb;

    Transform fire;
    public bool shipInRange { get; private set; }
    
    // Display booleans
    public bool boostActive { get; private set; }

    // Audio
    private AudioSource audioSource;
    public AudioClip boostSound;
    public AudioClip impact;
    public AudioClip door;
    public AudioClip whirSound;

    // Camera Stuff.
    private KeyCode switchCam = KeyCode.F3;
    public Camera firstPerson;
    public Camera thirdPerson;
    private bool isFirstPerson;

    private void Awake()
    {
        // Initialize attributes of the ship's rigidbody: Disable gravity and default physics, set interpolation, center of mass, and continuous collision detection.
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate; // Best result for rendering objects affected by physics.
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.centerOfMass = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        targetRotation = transform.rotation;
        smoothRotation = transform.rotation;
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.loop = true;
        isFirstPerson = false;
        thirdPerson.gameObject.SetActive(true);
        firstPerson.gameObject.SetActive(false);
    }
    private void Start()
    {
        // Start the game in the ship.
        hasPilot = true;
        fire = transform.Find("BlueFire");
    }

    // Update is called once per frame
    private void Update()
    {
        // Enable the ship if it has a pilot, provide opportunity to exit if grounded.
        CheckGrounded();
        if(hasPilot)
        {
            Drive();
            if(Input.GetKeyDown(escapeShip) && grounded == true)
            {
                ExitShip();
            }
        }
        else    // Otherwise, press F to enter the ship as a player.
        {
            if(Input.GetKeyDown(escapeShip) && shipInRange == true)
            {
                EnterShip();
            }
        }
        changeCamera();
    }

    private void FixedUpdate() 
    {
        // Apply custom gravity to ship.
        Vector3 gravity = NBodySimulation.CalculateAcceleration(rb.position);
        rb.AddForce(gravity, ForceMode.Acceleration);

        // Apply thruster force.
        Vector3 thrustDir = transform.TransformVector (thrust);
        rb.AddForce (thrustDir * thrustMultiplier, ForceMode.Acceleration);

        // Rotate while airborne
        if (numCollisionTouches == 0) 
        {
            rb.MoveRotation (smoothRotation);
        }

        // Check if the pilot is outside of the ship.
        if (FindObjectOfType<FirstPersonController>() != null)
        {
            // If so, check the distance between the ship and pilot.
            if (Vector3.Distance(FindObjectOfType<FirstPersonController>().transform.localPosition, transform.localPosition) < 3f)
            {
                shipInRange = true;
            }
            else
            {
                shipInRange = false;
            }
        }
    }

    private void Drive()
    {
        // Thrust Controllers (negative key, positive key).
        int thrustInputX = GetInputAxis(right, left);
        int thrustInputY = GetInputAxis(up, down);
        int thrustInputZ = GetInputAxis(backward, forward);
        int thrustBoost = 1;
        if(Input.GetKeyDown(boost) && !boostActive)
        {
            thrustBoost = 2;
            boostActive = true;
            audioSource.Stop();
            audioSource.clip = boostSound;
            audioSource.Play();
            audioSource.loop = true;
            fire.transform.localScale += new Vector3(1, 1, 1);
        }
        else if(Input.GetKeyDown(boost) && boostActive)
        {
            thrustBoost = 1;
            boostActive = false;
            audioSource.Stop();
            audioSource.clip = whirSound;
            audioSource.Play();
            audioSource.loop = true;
            fire.transform.localScale -= new Vector3(1, 1, 1);
        }

        if(Input.GetKey(slow))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        // Adjusts direction of ship depending on keys pressed above.
        thrust = new Vector3(thrustInputZ * thrustBoost, thrustInputY, thrustInputX); // Z and X are switched because I screwed up the directions of the solar system.

        // Rotation Controllers (negative key, positive key).
        float yawInput = Input.GetAxisRaw("Mouse X") * rotationSpeed; // Uses horizontal mouse input.
        float pitchInput = GetInputAxis(spinCounterClockwise, spinClockwise);
        float rollInput = Input.GetAxisRaw("Mouse Y") * rotationSpeed; // Uses vertical mouse input.

        // Calculate the rotation of the ship while it is airborne.
        if (numCollisionTouches == 0) 
        {
            // Get the yaw, pitch, and roll according to what the player inputs.
            var yaw = Quaternion.AngleAxis (yawInput, transform.up);
            var pitch = Quaternion.AngleAxis (pitchInput, transform.right);
            var roll = Quaternion.AngleAxis (rollInput, transform.forward);

            // Multiply yaw, pitch, and roll to get the resulting rotation.
            targetRotation = yaw * pitch * roll * targetRotation;

            // Smoothen the rotation through interpolation so it doesn't look jittery.
            smoothRotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * rotSmoothSpeed);
        } 
        else 
        {
            // Maintain current rotation if on the ground. (Preventing clipping and stuff)
            targetRotation = transform.rotation;
            smoothRotation = transform.rotation;
        }
    }

    // Determine whether the ship is turning left or right
    private int GetInputAxis(KeyCode negativeAxis, KeyCode positiveAxis) 
    {
        int axis = 0;

        // If the key corresponds to the positive axis, add to the current axis.
        if (Input.GetKey(positiveAxis)) 
        {
            axis++;
        }

        // If the key corresponds to the negative axis, subtract from the current axis.
        // No if else since we want both inputs to register.
        if (Input.GetKey(negativeAxis)) 
        {
            axis--;
        }

        return axis;
    }

    // Functions for exiting and entering the ship.
    // Switches cameras between ship and pilot.
    private void ExitShip()
    {
        // If player is on the planet, exit the ship.
        // Enable the pilot camera, disable the ship camera.
        //pilot.gameObject.GetComponentInChildren<Camera>().enabled = true;
        //transform.gameObject.GetComponentInChildren<Camera>().enabled = false;
        fire.gameObject.SetActive(false);
        // Create a new pilot gameobject.
        Instantiate(pilot, transform.localPosition, transform.localRotation);
        // Set its velocity equal to the stationary ship so the player does not fly off.
        FindObjectOfType<FirstPersonController>().GetComponent<Rigidbody>().velocity = rb.velocity;
        audioSource.Stop();
        audioSource.PlayOneShot(door);
        hasPilot = false;
    }

    private void EnterShip()
    {
        // If player is in the ship, enter the ship.
        // Disable the pilot camera, enable the ship camera.
        //pilot.gameObject.GetComponentInChildren<Camera>().enabled = false;
        //transform.gameObject.GetComponentInChildren<Camera>().enabled = true;
        fire.gameObject.SetActive(true);
        audioSource.PlayOneShot(door);
        audioSource.Play();
        Destroy(FindObjectOfType<FirstPersonController>().gameObject);
        hasPilot = true;
    }

    // For checking collisions
    private void OnCollisionEnter(Collision other) 
    {

        numCollisionTouches++;
        audioSource.PlayOneShot(impact, 0.6f);
    }

    private void OnCollisionExit(Collision other) 
    {
        numCollisionTouches = 0;
    }

    // Shoot a ray from the bottom of the ship and determine if there is ground below.
    private void CheckGrounded()
    {
        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 1 + 10f, groundedMask)) // max distance of 10f due to size of the ship
        {
            grounded = true;
        }

    }

        private void changeCamera()
    {
        // Change to third person.
        if(isFirstPerson && Input.GetKeyDown(switchCam))
        {
            isFirstPerson = false;
            shipCamera = thirdPerson.transform;
            firstPerson.gameObject.SetActive(false);
            thirdPerson.gameObject.SetActive(true);
        }
        else if(!isFirstPerson && Input.GetKeyDown(switchCam)) // Change to first person.
        {
            isFirstPerson = true;
            shipCamera = firstPerson.transform;
            firstPerson.gameObject.SetActive(true);
            thirdPerson.gameObject.SetActive(false);
        }
    }

}
