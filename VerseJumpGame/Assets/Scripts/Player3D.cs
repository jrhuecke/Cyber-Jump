using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3D : MonoBehaviour
{
    //Movement variables
    public CharacterController controller;
    public float speed;
    Vector3 velocity;
    private float forwardMovement, sidewaysMovement;

    //Jumping/falling variables
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;
    public float gravity = -9.81f;
    public float jumpPower;
    private bool isGrounded;

    void Update()
    {
        //checks if player is on ground by casting a sphere at players feet and seeing if that collides with the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //resets y velocity when player is on ground (so gravity doesn't constantly build up)
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpPower;
        }

        //moves player on x and z axis
        float sidewaysMovement = Input.GetAxis("Horizontal");
        float forwardMovement = Input.GetAxis("Vertical");
        Vector3 move = (transform.right * sidewaysMovement) + (transform.forward * forwardMovement);
        controller.Move(move * speed * Time.deltaTime);

        //Moves player on y axis
        controller.Move(velocity * Time.deltaTime);
    }
}
