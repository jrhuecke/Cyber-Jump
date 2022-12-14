using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player3D : MonoBehaviour
{
    enum State
    {
        WAITING,
        RUNNING,
        JUMPING,
        PRIMARY_FIRE,
        SECONDARY_WIND_UP,
        SECONDARY_FIRE,
        SCENE_START,
        DEAD
    }

    enum animState
    {
        WAITING,
        RUNNING,
        JUMPING,
    }

    State state;
    animState anim;

    //Audio
    public AudioSource audioSource;
    public AudioClip laserAudio;
    public AudioClip gunAudio;
    public AudioClip hitAudio;

    //Start of scene variables
    public float introLength;
    private float introTimer;
    public float introBufferLength;
    private float introBufferTimer;

    //Movement variables
    public CharacterController controller;
    public float speed;
    public Vector3 velocity, move;
    private float forwardMovement, sidewaysMovement;

    //Health variables
    public float playerHealth3D = 4f;
    public float bulletDamage = 1f;
    public float invulnerability = 1f;
    private float invulnerabilityTimer;
    public List<GameObject> hearts;
    public GameObject gameOvertext;

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
    public GameObject laserObject;
    public Transform bulletOrigin;
    public LayerMask environmentLayers;
    public Camera cam;

    public float bulletSpeed;
    public float bulletFireRate;
    private float bulletCooldownTimer;

    public float secondaryFireCharge;
    public float secondaryFireMaxCharge = 100f;
    public float secondaryFireWindUp = .5f;
    private float secondaryFireWindUpTimer;
    private float secondaryFireTimer;
    public float secondaryFireLength = 1.5f;
    public Transform chargeMeter;
    public GameObject crosshair;
    public GameObject chargeBar;

    //animation
    public Animator animator;

    private void Start()
    {
        introBufferTimer = 0f;
        bulletCooldownTimer = 0f;
        secondaryFireWindUpTimer = secondaryFireWindUp;
        state = State.SCENE_START;
        introTimer = 0f;
    }

    void Update()
    {
        //player can't do anything until intro is over
        if (state == State.SCENE_START)
        {
            if (introBufferTimer <= introBufferLength)
            {
                introBufferTimer += Time.deltaTime;
                return;
            }
            else
            {
                if (introTimer >= introLength)
                {
                    state = State.WAITING;
                }
                introTimer += Time.deltaTime;
                return;
            }
        }

        if (state == State.DEAD)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("3DScene");
            }
            return;
        }

        if (playerHealth3D <= 0)
        {
            gameOvertext.SetActive(true);
            laserObject.SetActive(false);
            chargeMeter.gameObject.SetActive(false);
            crosshair.SetActive(false);
            chargeBar.SetActive(false);
            state = State.DEAD;
            return;
        }

        //checks if player is on ground by casting a sphere at players feet and seeing if that collides with the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        applyGravity();

        //gathers input on if the player wants to jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        //gathers input for player on x and z axis
        sidewaysMovement = Input.GetAxis("Horizontal") * speed;
        forwardMovement = Input.GetAxis("Vertical") * speed;
        move = (transform.right * sidewaysMovement) + (transform.forward * forwardMovement);
        velocity = new Vector3(move.x, velocity.y, move.z);

        //Moves player (vertically and horizontally)
        if (velocity != Vector3.zero)
        {
            controller.Move(velocity * Time.deltaTime);
        }

        //teleports player back up to arena if they somehow fall through it
        if (transform.position.y < -5)
        {
            transform.position = new Vector3(0, 2, 0);
        }

        //cooldown between bullets
        if (bulletCooldownTimer > 0)
        {
            bulletCooldownTimer -= Time.deltaTime;
        }

        //Invuln time after taking damage cooldown
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }

        //updates charge meter UI
        if (secondaryFireCharge <= 22)
        {
            chargeMeter.localScale = new Vector3(0.4f + (secondaryFireCharge * 0.41f), 0.4f, 0.4f);
            chargeMeter.localPosition = new Vector3((-66 + (secondaryFireCharge * 3f)), -50, 0);
        }

        //state machine for animations
        switch(anim)
        {
            case animState.WAITING:
                if (velocity.y > 0)
                {
                    anim = animState.JUMPING;
                    animator.SetTrigger("Jumping");
                }
                else if (velocity.x != 0|| velocity.z != 0)
                {
                    anim = animState.RUNNING;
                    animator.SetTrigger("Running");
                }
                break;

            case animState.RUNNING:
                if (velocity.x == 0 && velocity.z == 0)
                {
                    anim = animState.WAITING;
                    animator.SetTrigger("Idle");
                }
                break;

            case animState.JUMPING:
                if (isGrounded)
                {
                    anim = animState.WAITING;
                    animator.SetTrigger("Idle");
                }
                break;
        }


        //state machine for player's shooting
        switch (state)
        {
            case State.WAITING:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    state = State.PRIMARY_FIRE;
                }
                else if (Input.GetKeyDown(KeyCode.Mouse1) && secondaryFireCharge >= secondaryFireMaxCharge)
                {
                    state = State.SECONDARY_WIND_UP;
                    secondaryFireCharge = 0;
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
                secondaryFireCharge = 0;
                if (secondaryFireWindUpTimer <= 0)
                {
                    secondaryFireWindUpTimer = secondaryFireWindUp;
                    secondaryFireTimer = secondaryFireLength;
                    laserObject.SetActive(true);
                    state = State.SECONDARY_FIRE;

                    //resets charge meter
                    chargeMeter.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                    chargeMeter.localPosition = new Vector3(-66, -50, 0);

                    audioSource.PlayOneShot(laserAudio);
                }
                break;

            case State.SECONDARY_FIRE:
                //Goes back to waiting when fire length is reached
                if (secondaryFireTimer <= 0)
                {
                    laserObject.SetActive(false);
                    secondaryFireCharge = 0;
                    state = State.WAITING;
                }
                secondaryFireCharge = 0;
                secondaryFireTimer -= Time.deltaTime;
                secondaryFire();
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
        //attaches the player3D script to the bullet, so it can access the secondary charge variable
        Bullet3D bulletScript = bullet.GetComponent<Bullet3D>();
        bulletScript.player3D = this;

        audioSource.PlayOneShot(gunAudio);
    }

    private void secondaryFire()
    {
        /* Fires out raycast to update where laser should be pointed every frame so
           that it actually points at crosshair */
        RaycastHit target;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out target, 100f, environmentLayers);
        if (target.point == Vector3.zero)
        {
            bulletOrigin.localRotation = Quaternion.Euler(0f, 0f, 0f);
            laserObject.transform.localPosition = new Vector3(0f, -.06f, 10f);
            laserObject.transform.localScale = new Vector3(0.2f, 10f, 0.2f);
        }
        else
        {
            /* Updates the local position and scale of laser so that it doesn't go through objects (it intentionally pokes a tiny bit into objects
               so that damage collision can be detected) */
            bulletOrigin.LookAt(target.point);
            laserObject.transform.localPosition = new Vector3(0f, -.06f, Mathf.Abs((Vector3.Distance(bulletOrigin.position, target.point)) + 0.5f) / 2);
            laserObject.transform.localScale = new Vector3(0.2f, Mathf.Abs((Vector3.Distance(bulletOrigin.position, target.point)) + 0.5f) / 2, 0.2f); 
        }
    }

    //Checks for boss attacks hitting player
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.layer == 9 || other.gameObject.layer == 7) && invulnerabilityTimer <= 0)
        {
            invulnerabilityTimer = invulnerability;
            playerHealth3D -= bulletDamage;
            if (playerHealth3D >= 0)
            {
                hearts[Mathf.FloorToInt(playerHealth3D)].SetActive(false);

                audioSource.PlayOneShot(hitAudio);
            } 
        }
    }
}
