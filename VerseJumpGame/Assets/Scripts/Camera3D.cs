using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3D : MonoBehaviour
{
    public Transform playerTF;
    public Transform followTarget;
    public float horizontalSensitivity;
    public float verticalSensitivity;
    private float mouseX, mouseY;
    private float xRotation;

    private void Start()
    {
        xRotation = 0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        //Getting mouse input
        mouseX = Input.GetAxis("Mouse X") * horizontalSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity * Time.deltaTime;

        //looking left/right (rotates the player and follow target the camera is tracking)
        playerTF.Rotate(Vector3.up * mouseX);

        //looking up and down (only rotates the follow target the camera is tracking)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        followTarget.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}