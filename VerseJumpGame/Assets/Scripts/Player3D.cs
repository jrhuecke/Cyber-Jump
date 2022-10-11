using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3D : MonoBehaviour
{
    enum State
    {
        WAITING,
        PRIMARY_FIRE,
        SECONDARY_WIND_UP,
        SECONDARY_FIRE
    }

    State state;

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
    public GameObject laserPrefab;
    public Transform bulletOrigin;
    public Camera cam;

    public float bulletSpeed;
    public float bulletFireRate;
    private float bulletCooldownTimer;

    public float secondaryFireCharge;
    public float secondaryFireMaxCharge = 100f;
    public float secondaryFireWindUp = .5f;
    public float secondaryFireWindUpTimer;
    private bool usingSecondaryFire;
    public float secondaryFireTimer;
    public float secondaryFireLength = 1.5f;
    

    private void Start()
    {
        bulletCooldownTimer = 0f;
        secondaryFireWindUpTimer = secondaryFireWindUp;
        usingSecondaryFire = false;
        state = State.WAITING;
    }

    void Update()
    {
        //checks if player is on ground by casting a sphere at players feet and seeing if that collides with the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        applyGravity();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
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

        if (bulletCooldownTimer > 0)
        {
            bulletCooldownTimer -= Time.deltaTime;
        }

        switch (state)
        {
            case State.WAITING:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    state = State.PRIMARY_FIRE;
                }
                break;

            case State.PRIMARY_FIRE:
                //Shoots bullets at a set fire rate while button is held down
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    if (bulletCooldownTimer <= 0)
                    {
                        bulletCooldownTimer = bulletFireRate;
                        primaryFire();
                    }
                } 
                else
                {
                    state = State.WAITING;
                }
                break;

            case State.SECONDARY_WIND_UP:
                //Charge up time before secondary fire goes off
                secondaryFireWindUpTimer -= Time.deltaTime;
                if (secondaryFireWindUpTimer <= 0)
                {
                    secondaryFireWindUpTimer = secondaryFireWindUp;
                    secondaryFireTimer = secondaryFireLength;
                    laserPrefab.SetActive(true);
                    state = State.SECONDARY_FIRE;
                }
                break;

            case State.SECONDARY_FIRE:
                //Goes back to waiting when fire length is reached
                if (secondaryFireTimer <= 0)
                {
                    state = State.WAITING;
                }
                /* Fires out raycast to update where laser should be pointed every frame so
                  that it actually points at crosshair */
                secondaryFireTimer -= Time.deltaTime;
                RaycastHit target;
                Physics.Raycast(cam.transform.position, cam.transform.forward, out target);
                if (target.point == Vector3.zero)
                {
                    bulletOrigin.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
                else
                {
                    bulletOrigin.LookAt(target.point);
                }
                break;
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

    private void primaryFire()
    {
        /* Before shooting, sends out a raycast from the camera to see what the crosshair is currently
          on and then rotates the bullet origin so the bullet shoots at where the crosshair was pointed. */
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

    private void secondaryFire()
    {

    }
}
