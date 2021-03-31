using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Controller that is responsible for the player movement.
 * Ensures that the player can only jump when touching the ground. Also limits camera movement and specifies player walk speed.
 * 1/31/2021
 */
public class FirstPersonController : MonoBehaviour
{
    // Declare instances.
    // Mouse sensitivity values.
    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;

    // Movement values, walk and jump.
    public float walkSpeed = 4f;
    public float jumpForce = 220f;

    // Boolean for checking if player is touching the ground.
    bool grounded;
    private bool jumping;
    private bool belowGround;
    // Camera Reference.
    Transform cameraT;

    // Value of camera's vertical rotation.
    float verticalLookRotation;

    // Ground Layer to check if player is standing on an object.
    public LayerMask groundedMask;

    // FPS Movement Instances.
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;
    private Vector3 targetMoveAmount;

    // Audio Stuff.
    private AudioSource audioSource;
    public AudioClip jump;
    public AudioClip land;
    // Animation Stuff.
    private Animator anim;
    private KeyCode switchCam = KeyCode.F3;

    // Camera Stuff.
    public Camera firstPerson;
    public Camera thirdPerson;
    private bool isFirstPerson;

    // On first frame.
    private void Awake()
    {
        cameraT = firstPerson.transform;
        thirdPerson.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
		anim = gameObject.GetComponentInChildren<Animator>();
        isFirstPerson = true;
    }

    // Update is called once per frame.
    void Update()
    {
        // Transform the camera on the X axis when the mouse is moved horizontally.
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivityX);

        // Assign the value of the camera's vertical rotation to mouse input.
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivityY;

        // Limit the vertical and horizontal rotation.
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -80, 80);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;
        
        // Calculate movement when key is pressed.
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        targetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);
        changeCamera();
        // Jump Controller, jump is space is pressed.
        if (Input.GetButtonDown("Jump") && grounded == true)
        {
            GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
            jumping = true;
            audioSource.PlayOneShot(jump, 0.3f);
        }
        // Check if the player is on the ground before jumping.
        /*
        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
        {
            grounded = true;
        }
        */
        // Note: For the third parameter of raycast, check the model of the player to correctly assign the base, otherwise the script will incorrectly determine grounded.
        // Check for collider, grounded check is typically in the center, so divide the size of the collider by 2, and add a small value to compensate for error.
    }
    
    // If the player is not moving during a jump and the audio player is not playing any sound, play the landing clip.
    private void OnCollisionEnter(Collision other) 
    {
        if(!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(land);
        }
        jumping = false;
    }

    // For each frame a collision occurs.
    private void OnCollisionStay(Collision other) 
    {
        // Play the walking sound only when the player is moving and when the currently playing clip is finished.
        // Play walking animation,
        if(targetMoveAmount.magnitude > 0f)
        {
            if(!audioSource.isPlaying)
            {
                audioSource.PlayDelayed(0.2f);
            }
            anim.SetInteger ("AnimationPar", 1);
        }
        else
        {
            anim.SetInteger ("AnimationPar", 0);
        }
        grounded = true;
    }

    // When a collision is not detected.
    private void OnCollisionExit(Collision other) 
    {
        grounded = false;
    }

    // Move the player's rigidbody using the calculated movement.
    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    private void changeCamera()
    {
        // Change to third person.
        if(isFirstPerson && Input.GetKeyDown(switchCam))
        {
            isFirstPerson = false;
            cameraT = thirdPerson.transform;
            firstPerson.gameObject.SetActive(false);
            thirdPerson.gameObject.SetActive(true);
        }
        else if(!isFirstPerson && Input.GetKeyDown(switchCam)) // Change to first person.
        {
            isFirstPerson = true;
            cameraT = firstPerson.transform;
            firstPerson.gameObject.SetActive(true);
            thirdPerson.gameObject.SetActive(false);
        }
    }

}
