using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 0.5f;
    public float verticalLimit = 60f;

    private Transform playerTransform;
    private Quaternion originalRotation;
    private float currentVerticalRotation = 0f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Store the player's transform and the camera's original rotation
        playerTransform = transform.parent;
        originalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        MouseLook();
    }

    void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Update the camera's rotation using a quaternion
        currentVerticalRotation -= mouseY * mouseSensitivity;
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, -verticalLimit, verticalLimit);
        Quaternion xRot = Quaternion.AngleAxis(currentVerticalRotation, Vector3.right);
        Quaternion yRot = Quaternion.AngleAxis(mouseX * mouseSensitivity, Vector3.up);
        Quaternion newRotation = originalRotation * xRot * yRot;

        // Rotate the camera using a quaternion
        transform.localRotation = newRotation;

        // Rotate the player object using a quaternion
        Quaternion playerRotation = Quaternion.AngleAxis(mouseX * mouseSensitivity, Vector3.up);
        playerTransform.localRotation *= playerRotation;
    }
}
