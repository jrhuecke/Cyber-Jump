using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3D : MonoBehaviour
{
    //Movement variables
    public CharacterController controller;
    public float speed;
    public Vector3 velocity, move;
    private float forwardMovement, sidewaysMovement;

    //Jumping/falling variables
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;
    public float gravity = -9.81f;
    public float jumpPower;
    public float fallMultiplier = 1.25f;
    private bool isGrounded;

    //Gun variables
    public Rigidbody bulletPrefab;
    public Transform bulletOrigin;
    public Camera cam;
    public float bulletSpeed;

    void Update()
    {
        //checks if player is on ground by casting a sphere at players feet and seeing if that collides with the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        applyGravity();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire();
        }

        //moves player on x and z axis
        sidewaysMovement = Input.GetAxis("Horizontal") * speed;
        forwardMovement = Input.GetAxis("Vertical") * speed;
        move = (transform.right * sidewaysMovement) + (transform.forward * forwardMovement);
        velocity = new Vector3(move.x, velocity.y, move.z);

        //Moves player
        if (velocity != Vector3.zero)
        {
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void applyGravity()
    {
        //resets y velocity when player is on ground (so gravity doesn't constantly build up)
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        //applies gravity when player is in the air
        else if (!isGrounded && velocity.y > 0)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        //applies greater gravity when player is falling to make jumping feel less floaty
        else
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
    }

    private void Jump()
    {
        velocity.y = jumpPower;
    }

    private void Fire()
    {
        /*Before shooting, sends out a raycast from the camera to see what the crosshair is currently
          on and then rotates the bullet origin so the bullet shoots at where the crosshair was pointed.*/
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit);
        if (hit.point == Vector3.zero)
        {
            bulletOrigin.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            bulletOrigin.LookAt(hit.point);
        }
        Rigidbody bullet = Instantiate(bulletPrefab, bulletOrigin.position, bulletOrigin.rotation);
        bullet.velocity = bulletOrigin.forward * bulletSpeed;
    }
}
