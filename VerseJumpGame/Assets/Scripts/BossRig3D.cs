using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/*my idea is that each attack will be a function.
the attacks can be placed in a queue via name/id number and called in order.
update will pop off the queue every time the boss is able to perform an attack, and will execute that attack
*/

public class BossRig3D : MonoBehaviour
{
    private System.Random rng = new System.Random();
    private enum Attack{
        GUN, 
        CHARGE,
        SPIN, 
        MISSILE,
        LASER
    };

    //Intro variables
    public float introLength;
    private float introTimer;
    public float introBufferLength;
    private float introBufferTimer;

    //UI
    public Transform healthMeter;

    private Queue<Attack> bossQ;
    [SerializeField] float bossMaxHealth = 1000;
    public float bossCurrHealth;
    [SerializeField] float playerBulletDamage = 2;
    [SerializeField] float playerLaserDamage = 1;
    [SerializeField] float attackRate = 3.0f; //at minimum 3 sec delay between attacks
    private float nextAttack; //used to calculate when boss can begin next attack
    private bool attacking = false; //is the boss currently in an attack animation
    private bool passiveTracking = true; //boss constantly turns to look at player if true.

    [SerializeField] int gunShots = 3; //# of machine gun shots
    [SerializeField] float bulletSpeed = 30f; //machine gun bullet speed
    [SerializeField] float shotInterval = 0.3f;
    private int fired; //track bullets fired and stop if it is >= gunshots

   
    private float spinTime = 0f;
    [SerializeField] float spinDuration = 1f;
    private float spinSpeed;

    private float chargeTime = 0f;
    private Transform targetDest;
    private float dist;
    [SerializeField] float boostHeight = 3f;
    [SerializeField] float boostDuration = 0.5f;
    [SerializeField] float chargeDuration = 2f;

    private float laserTime = 0f;
    [SerializeField] float laserChargeDuration = 1f;

    public GameObject bulletPrefab;
    private Collider bulletColl;
    private Rigidbody bulletRB;
    public GameObject player;
    public GameObject gunOrigin;
    public GameObject laserOrigin;
    public GameObject swordHitbox;
    public GameObject laser;
    public GameObject laserCharge;
    public GameObject weakPoint;
    public Animator animator;

    private enum State {
        CHARGE,
        BOOST_UP,
        SPIN,
        LASER_CHARGE,
        LASER_FIRE,
        IDLE,
        SCENE_START
    };

    State state;

    // Start is called before the first frame update
    void Start()
    {
        introTimer = 0;
        introBufferTimer = 0;

        bossQ = new Queue<Attack>();
        fired = 0;

        bulletRB = bulletPrefab.GetComponent<Rigidbody>();
        bulletColl = bulletPrefab.GetComponent<Collider>();

        //Physics.IgnoreCollision(bulletColl, gameObject.GetComponent<Collider>());
        spinSpeed = (360/spinDuration);
        Debug.Log(spinSpeed);

        state = State.SCENE_START;

        laser.SetActive(false);
        laserCharge.SetActive(false);
        swordHitbox.SetActive(false);

        bossCurrHealth = bossMaxHealth;

        animator.SetTrigger("Idle");

    }

    // Update is called once per frame
    void Update()
    {

        //boss can't do anything until intro is over
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
                    state = State.IDLE;
                }
                introTimer += Time.deltaTime;
                return;
            }  
        }

        //boss looking at player when enabled
        if (passiveTracking) {gameObject.transform.LookAt(player.transform.position);}

        //Updates boss health UI
        if (bossCurrHealth >= 0)
        {
            healthMeter.localScale = new Vector3((.119f * bossCurrHealth) + 5, 5, 5);
            healthMeter.localPosition = new Vector3((.238f * bossCurrHealth) - 238, -500, 0);
        }
        else
        {
            SceneManager.LoadScene("EndDialogueScene");
        }


        //boss attack state machine
        switch (state) {
            default:
                state = State.IDLE;
                break;

            case State.IDLE:
                break;

            case State.CHARGE:
                if(Time.time <= chargeTime + boostDuration + chargeDuration) {
                    gameObject.transform.Translate(Vector3.forward * Time.deltaTime * (dist/chargeDuration));
                    animator.SetTrigger("Flying");
                }
                else{
                    state = State.IDLE;
                    attacking = false;
                    gameObject.transform.SetPositionAndRotation(new Vector3(
                        gameObject.transform.position.x, 3, gameObject.transform.position.z),
                        gameObject.transform.rotation);
                    passiveTracking = true;
                    animator.SetTrigger("Idle");
                    animator.ResetTrigger("Flying");
                }
                break;

            case State.BOOST_UP:
                if(Time.time <= chargeTime + boostDuration) {
                    gameObject.transform.Translate(Vector3.up * Time.deltaTime * (boostHeight/boostDuration), Space.World);
                    animator.SetTrigger("Flying");
                }
                else {
                    state = State.CHARGE;
                    dist = Vector3.Distance(targetDest.position, gameObject.transform.position);
                    animator.SetTrigger("Flying");
                }
                break;

            case State.SPIN:
                swordHitbox.SetActive(true);
                gameObject.transform.RotateAround(gameObject.transform.position, Vector3.up, spinSpeed * Time.deltaTime);
                if (Time.time >= spinTime + spinDuration) {
                    state = State.IDLE;
                    attacking = false;
                    passiveTracking = true;
                    swordHitbox.SetActive(false);
                    animator.ResetTrigger("Sword");
                    animator.SetTrigger("Idle");
                }
                break;

            case State.LASER_CHARGE:
                laserCharge.SetActive(true);
                if(Time.time < laserTime + (laserChargeDuration/3)) {
                    laserCharge.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                }
                else if(Time.time >= laserTime + (laserChargeDuration/3) && Time.time < laserTime + 2*(laserChargeDuration/3)) {
                    laserCharge.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                }
                else {
                    laserCharge.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                }
                if(Time.time >= laserTime + laserChargeDuration) {
                    laserCharge.SetActive(false);
                    state = State.LASER_FIRE;
                }
                break;
                
            case State.LASER_FIRE:
                if(Time.time < laserTime + laserChargeDuration + 0.35f) {
                    laser.SetActive(true);
                }
                else {
                    laser.SetActive(false);
                    state = State.IDLE;
                    attacking = false;
                }
                break;
        }


        //tracking projectile origin to aim at player (happens always)
        gunOrigin.transform.LookAt(player.transform.position);
        laserOrigin.transform.LookAt(player.transform.position);

        //execute the next attack in the queue
        if(!attacking) {
            if (bossQ.Count > 0) {
                attacking = true;
                nextAttack = Time.time + attackRate;
                Attack curr = bossQ.Dequeue();
                switch(curr) {
                    case Attack.GUN:
                        Debug.Log("Gun!");
                        fired = 0;
                        InvokeRepeating("Gun", 0, shotInterval);
                        animator.SetTrigger("Shooting");
                        break;
                    case Attack.CHARGE:
                        Charge();
                        break;
                    case Attack.SPIN:
                        Spin();
                        break;
                    case Attack.MISSILE:
                        Missiles();
                        break;
                    case Attack.LASER:
                        Laser();
                        break;
                    default: 
                        Debug.Log("Failed to find attack function");
                        break;
                }
            }
        }

        //randomly select the next attack
        if(!attacking && Time.time > nextAttack) {
            SelectAttack();
        }
    }

    public void OnNormTriggerEnter(OnTriggerDelegation delegation) {
        if (delegation.Other.gameObject.layer == 8) {
            bossCurrHealth -= playerBulletDamage;
            Debug.Log("Damage taken! HP: " + bossCurrHealth + "/" + bossMaxHealth);
        }
    }

    public void OnWeakTriggerEnter(OnTriggerDelegation delegation) {
        bossCurrHealth -= playerBulletDamage *2;
        Debug.Log("Weak Point Hit! HP: " + bossCurrHealth+ "/" + bossMaxHealth);

        if(state == State.LASER_CHARGE) {
            state = State.IDLE;
            laserCharge.SetActive(false);
            laser.SetActive(false);
            attacking = false;
            Debug.Log("LASER CHARGE CANCELLED!");
            }
    }

    public void OnNormTriggerStay(OnTriggerDelegation delegation)
    {
        if (delegation.Other.gameObject.layer == 11)
        {
            bossCurrHealth -= playerLaserDamage * Time.deltaTime;
            Debug.Log("Damage taken! HP: " + bossCurrHealth + "/" + bossMaxHealth);
        }
    }

    public void OnWeakTriggerStay(OnTriggerDelegation delegation)
    {
        if (delegation.Other.gameObject.layer == 11)
        {
            bossCurrHealth -= playerLaserDamage * Time.deltaTime * 2;
            Debug.Log("Weak Spot Hit! HP: " + bossCurrHealth + "/" + bossMaxHealth);
            if (state == State.LASER_CHARGE)
            {
                state = State.IDLE;
                laserCharge.SetActive(false);
                laser.SetActive(false);
                attacking = false;
                Debug.Log("LASER CHARGE CANCELLED!");
            }
        }
    }





    //attack selection ai
    void SelectAttack() {
        int pick = rng.Next(1,100);

        if(pick < 55) {
            Debug.Log(Vector3.Distance(gameObject.transform.position, player.transform.position));
            if(Vector3.Distance(gameObject.transform.position, player.transform.position) < 6.8f) {
                bossQ.Enqueue(Attack.SPIN);
            }
            else{
                bossQ.Enqueue(Attack.GUN);
            }
        }
        else if(pick >= 55 && pick < 75) {
            bossQ.Enqueue(Attack.CHARGE);
            bossQ.Enqueue(Attack.SPIN);
        }
        else if(pick >= 75 && pick < 92) {
            Debug.Log(Vector3.Distance(gameObject.transform.position, player.transform.position));
            if(Vector3.Distance(gameObject.transform.position, player.transform.position) < 6.8f) {
                bossQ.Enqueue(Attack.SPIN);
            }
            else{
                bossQ.Enqueue(Attack.LASER);
            }
        
        }
        else {
            bossQ.Enqueue(Attack.CHARGE);
            bossQ.Enqueue(Attack.SPIN);
        }
    }

    void Gun(){
        if(fired < gunShots) {
            Rigidbody bulletInstance = Instantiate(bulletRB, gunOrigin.transform.position, gunOrigin.transform.rotation);
            bulletInstance.velocity = gunOrigin.transform.forward * bulletSpeed;
            attacking = true;
            fired += 1;
            animator.SetTrigger("Shooting");
        }
        else {
            CancelInvoke("Gun");
            attacking = false;
            animator.SetTrigger("Idle");
            animator.ResetTrigger("Shooting");
        }
    }

    void Charge(){
        Debug.Log("Charge!");
        attacking = true;
        state = State.BOOST_UP;
        chargeTime = Time.time;
        passiveTracking = false;
        targetDest = player.transform;
        gameObject.transform.LookAt(player.transform.position);
        animator.SetTrigger("Flying");
        
    }

    void Spin(){
        Debug.Log("Spin!");
        passiveTracking = false;
        spinTime = Time.time;
        state = State.SPIN;
        animator.SetTrigger("Sword");
        
    }

    void Missiles(){
        Debug.Log("Missiles!");
        attacking = false;
    }

    void Laser(){
        Debug.Log("Laser!");
        attacking = true;
        laserTime = Time.time;
        state = State.LASER_CHARGE;
    }

}
