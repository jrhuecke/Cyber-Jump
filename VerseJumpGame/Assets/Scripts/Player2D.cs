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

public class Player2D : MonoBehaviour
{
    // Start is called before the first frame update
    public float speedMax = 8.0f;
    Vector2 moveInput = Vector2.zero;
    Rigidbody2D rigidbody;
    SpriteRenderer sprite;

    Vector2 mousePos;
    public GameObject crosshair;
    public GameObject weaponRoot;
    public GameObject projectileOrigin;

    [Header("On-Hit stuff")]
    public float iFrames = 1.0f;
    float endiFrames = 0.0f;

    [Header("Machine Gun bullets")]
    public Rigidbody2D bulletPrefab;
    public int bulletDamage = 10;
    public float bulletSpeed = 16.0f;

    void Start()
    {
        Cursor.visible = false;
        rigidbody = this.GetComponent<Rigidbody2D>();
        sprite = this.GetComponent<SpriteRenderer>();
    }

    /*The player input component sends these function calls/"messages" to this script
      The various function calls are listed on the component itself.*/
    void OnFire()
    {
        Cursor.visible = false;

        Rigidbody2D bulletInstance = Instantiate(bulletPrefab, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
        bulletInstance.velocity = weaponRoot.transform.right * bulletSpeed;
        bulletInstance.gameObject.GetComponent<DamageBoss2D>().DamageToBoss = bulletDamage;
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
        rigidbody.velocity = conversion * speedMax;

        //Animation stuff
        if (crosshair.transform.position.x > gameObject.transform.position.x)
        {
            sprite.flipX = false;
            weaponRoot.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            sprite.flipX = true;
            weaponRoot.transform.localScale = new Vector3(1, -1, 1);
        }

        //Gun + projectile rotation
        Vector3 crosshair2Dpos = crosshair.transform.position;
        crosshair2Dpos.z = 0;
        weaponRoot.transform.right = crosshair2Dpos - gameObject.transform.position;

        /*
        if(moveInput != Vector2.zero && !attacking) //If player stops moving, the direction they were facing will persist instead of being reset
        {
            charAnimator.SetFloat("VelocityX", rigidbody.velocity.x);
            charAnimator.SetFloat("VelocityY", rigidbody.velocity.y);
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

    //To be called by trigger colliders that are meant to deal damage to the player (bullets, boss melee, etc)
    public void TakeDamage(int damage)
    {
        if(damage > -1 && Time.time >= endiFrames)
        {
            Debug.Log("Player took " + damage + " damage!");
            endiFrames = Time.time + iFrames;
        }
        else if(damage < 0)
        {
            Debug.Log("Player took " + damage + " damage!");
        }
    }
}
