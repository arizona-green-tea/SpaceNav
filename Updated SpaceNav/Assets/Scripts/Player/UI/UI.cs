using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI speedDisplay;
    public TextMeshProUGUI boostDisplay;
    public TextMeshProUGUI refPlanetDisplay;
    public ShipController ship;
    public GameObject shipUI;
    public TextMeshProUGUI exitUI;
    public TextMeshProUGUI enterUI;
    public TextMeshProUGUI dstDisplay;
    public TextMeshProUGUI playerPlanetDisplay;
    public TextMeshProUGUI ascendingDisplay;
    public GameObject playerUI;
    private FirstPersonController player;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        exitUI.enabled = false;
        enterUI.enabled = false;
    }

    void Start()
    {
        updateShipUI();
    }

    // Update is called once per frame
    void Update()
    {
        player = FindObjectOfType<FirstPersonController>();
        if(ship.hasPilot)
        {
            shipUI.SetActive(true);
            playerUI.SetActive(false);
            updateShipUI();
        }
        else
        {
            shipUI.SetActive(false);
            playerUI.SetActive(true);
            updatePlayerUI();
        }
        updateExitUI();
        updateEnterUI();

        if(Input.GetKey(KeyCode.Space))
        {
            ascendingDisplay.text = "Descending";
        }
        else if(Input.GetKey(KeyCode.LeftShift)) 
        {
            ascendingDisplay.text = "Ascending";
        }
        else
        {
            ascendingDisplay.text = "Neutral";
        }
    }

    private void updateShipUI()
    {
        boostDisplay.text = "Boost Active: " + ship.boostActive;
        if(ship.GetComponent<GravityBody>().referenceBody != null)
        {
            refPlanetDisplay.text = "Current Body: " + ship.GetComponent<GravityBody>().referenceBody.getName();
        }
        if(ship.grounded && (Mathf.RoundToInt(ship.GetComponent<GravityBody>().referenceBody.velocity.magnitude - 
            ship.GetComponent<Rigidbody>().velocity.magnitude) >= 0))
        {
            speedDisplay.text = "Speed: " + (Mathf.RoundToInt(ship.GetComponent<GravityBody>().referenceBody.velocity.magnitude - 
            ship.GetComponent<Rigidbody>().velocity.magnitude) + "m/s");
        }
        else
        {
            speedDisplay.text = "Speed: " + Mathf.RoundToInt(ship.GetComponent<Rigidbody>().velocity.magnitude) + "m/s";
        }
    }

    private void updateExitUI()
    {
        if(ship.grounded && ship.hasPilot)
        {
            exitUI.enabled = true;
        }
        else
        {
            exitUI.enabled = false;
        }
    }

    private void updateEnterUI()
    {
        if(ship.shipInRange && !ship.hasPilot)
        {
            enterUI.enabled = true;
        }
        else
        {
            enterUI.enabled = false;
        }
    }

    private void updatePlayerUI()
    {
        dstDisplay.text = "Distance to Ship: " + 
        Mathf.RoundToInt(Vector3.Distance(player.transform.position, ship.transform.position)) + "m";
        if (player.GetComponent<GravityBody>().referenceBody != null)
        {
            playerPlanetDisplay.text = "Currently Exploring: " + player.GetComponent<GravityBody>().referenceBody.getName();
        }
    }
}
