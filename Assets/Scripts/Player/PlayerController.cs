using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Connection;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour, IInitialize
{
    /////////////////////////////////////////////////////////////////////////////////////
    // PUBLIC VARIABLES
    /////////////////////////////////////////////////////////////////////////////////////

    public float speed = 5f;

    /////////////////////////////////////////////////////////////////////////////////////
    // PRIVATE VARIABLES
    /////////////////////////////////////////////////////////////////////////////////////

    private float gravity = -9.81f * 100.0f;
    private CharacterController characterController;
    private Vector3 velocity = Vector3.zero;

    public int owner = 0;

    public bool isActive { get; set; } = false;

    /////////////////////////////////////////////////////////////////////////////////////
    // START
    /////////////////////////////////////////////////////////////////////////////////////

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("IsOwner: " + IsOwner + " IsServer: " + NetworkManager.IsServer + " ClientId" + LocalConnection.ClientId);
        if( IsOwner == true )
            Initialize();
        else
            Deinitialize();
    }

    public void Initialize()
    {   
        if (IsOwner == false)
        {
            Debug.Log("Deinitialize");
            return;
        }

        characterController = GetComponent<CharacterController>();
        GameObject cameraObj = transform.GetChild(0).gameObject;
        cameraObj.GetComponent<CameraController>().Initialize();
        cameraObj.GetComponent<PlayerUsable>().Initialize();
        isActive = true;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // DEINITIALIZE
    /////////////////////////////////////////////////////////////////////////////////////

    public void Deinitialize()
    {
        GameObject cameraObj = transform.GetChild(0).gameObject;
        // cameraObj.GetComponent<PlayerUsable>().Deinitialize();
        // cameraObj.GetComponent<AudioListener>().enabled = false;
        // cameraObj.SetActive(false);
        Destroy(cameraObj);
        isActive = false;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // UPDATE
    /////////////////////////////////////////////////////////////////////////////////////

    void Update()
    {   
        if( isActive == false || IsOwner == false )
            return;

        Movement();
    }

    /////////////////////////////////////////////////////////////////////////////////////
    // MOVEMENT
    /////////////////////////////////////////////////////////////////////////////////////

    void Movement()
    {
        velocity = Vector3.zero; // Reset velocity each frame

        if (Input.GetKey(KeyCode.W))
        {
            velocity += transform.forward * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity -= transform.forward * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            velocity -= transform.right * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity += transform.right * speed;
        }

        // Apply gravity
        if (characterController.isGrounded)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}
