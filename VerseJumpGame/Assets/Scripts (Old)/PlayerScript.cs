using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 NOTE: Some code here for the animator is based on an older sprite I was using that had multiple rotations,
and thus currently does nothing.
But I've left the code here in case we DO want a sprite with rotations - so we can bring it back

I'd also prefer to use a Kinematic body, but I can't get it to collide with any other bodies, even when
useFullKinematicContacts is set to true. Maybe we can make this change later?
 */

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float speedMax = 8.0f;
    Vector2 moveInput = Vector2.zero;
    Rigidbody2D defaultController;
    Animator charAnimator;
    SpriteRenderer sprite;

    Vector2 mousePos;
    public GameObject crosshair;
    public GameObject weaponRoot;
    public GameObject projectileOrigin;

    public float attackDuration = 0.125f;
    float attackCooldown = 0f;
    bool attacking = false;

    public Rigidbody2D bulletPrefab;
    public float bulletSpeed = 16.0f;

    void Start()
    {
        Cursor.visible = false;
        defaultController = this.GetComponent<Rigidbody2D>();
        charAnimator = this.GetComponent<Animator>();
        sprite = this.GetComponent<SpriteRenderer>();
    }

    /*The player input component sends these function calls/"messages" to this script
      The various function calls are listed on the component itself.*/
    void OnFire()
    {
        //Debug.Log("Fire!");
        Cursor.visible = false;

        attacking = true;
        attackCooldown = attackDuration;

        Vector3 attackDirection = crosshair.transform.position - gameObject.transform.position;
        attackDirection = attackDirection.normalized;

        charAnimator.SetFloat("VelocityX", attackDirection.x);
        charAnimator.SetFloat("VelocityY", attackDirection.y);

        Rigidbody2D bulletInstance = Instantiate(bulletPrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
        bulletInstance.velocity = weaponRoot.transform.forward * bulletSpeed;
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    /*Alt method of controlling crosshair: Make a second Input Action Asset (since I think each one is a player?)*/
    void OnAim(InputValue value)
    {
        mousePos = value.Get<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 converion = new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane);
        crosshair.transform.position = Camera.main.ScreenToWorldPoint(converion);
    }

    private void FixedUpdate()
    {
        Vector3 conversion = new Vector3(moveInput.x, moveInput.y);
        conversion = conversion.normalized;
        defaultController.velocity = conversion * speedMax;

        //Animation stuff
        charAnimator.SetFloat("Speed", defaultController.velocity.magnitude);
        if(crosshair.transform.position.x > gameObject.transform.position.x)
        {
            sprite.flipX = false;
        }
        else
        {
            sprite.flipX = true;
        }

        //Gun + projectile rotation
        Vector2 aimDirection = new Vector2(crosshair.transform.position.x - gameObject.transform.position.x, crosshair.transform.position.y - gameObject.transform.position.y);
        float aimRotation = Vector2.Angle(Vector2.zero, aimDirection); //Returns 0 always?
        weaponRoot.transform.LookAt(new Vector2(crosshair.transform.position.x, crosshair.transform.position.y));


        //Looking at the aim direction (which is crosshair position offset by player position), seems to make the pistol disappear. Does lookAt use world positions instead?
        //The Z-axis of the pistol sprite is considered the 'face' that is looking at the position.

        /*
        if(moveInput != Vector2.zero && !attacking) //If player stops moving, the direction they were facing will persist instead of being reset
        {
            charAnimator.SetFloat("VelocityX", defaultController.velocity.x);
            charAnimator.SetFloat("VelocityY", defaultController.velocity.y);
        }

        //Attack animation
        attackCooldown -= Time.fixedDeltaTime;
        if(attackCooldown < 0f)
        {
            attacking = false;
            Debug.Log("Not attacking");
        }
        charAnimator.SetBool("Attacking", attacking);
        */
    }
}
