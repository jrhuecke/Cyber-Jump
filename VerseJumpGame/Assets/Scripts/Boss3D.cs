using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*my idea is that each attack will be a function.
the attacks can be placed in a queue via name/id number and called in order.
update will pop off the queue every time the boss is able to perform an attack, and will execute that attack
*/

public class Boss3D : MonoBehaviour
{
    private System.Random rng = new System.Random();
    private enum Attack{
        GUN, 
        CHARGE,
        SPIN, 
        MISSILE,
        LASER
    };

    private Queue<Attack> bossQ;
    [SerializeField] int bossMaxHealth = 1000;
    private int bossCurrHealth;
    [SerializeField] int playerBulletDamage = 2;
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
    public GameObject projectileOrigin;
    public GameObject sword;
    public GameObject swordHitbox;
    public GameObject laser;
    public GameObject laserCharge;
    public GameObject weakPoint;

    private enum State {
        CHARGE,
        BOOST_UP,
        SPIN,
        LASER_CHARGE,
        LASER_FIRE,
        IDLE
    };

    State state;

    // Start is called before the first frame update
    void Start()
    {
        bossQ = new Queue<Attack>();
        fired = 0;

        bulletRB = bulletPrefab.GetComponent<Rigidbody>();
        bulletColl = bulletPrefab.GetComponent<Collider>();

        //Physics.IgnoreCollision(bulletColl, gameObject.GetComponent<Collider>());
        spinSpeed = (360/spinDuration);
        Debug.Log(spinSpeed);

        state = State.IDLE;

        laser.SetActive(false);
        laserCharge.SetActive(false);
        swordHitbox.SetActive(false);

        bossCurrHealth = bossMaxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        //boss looking at player when enabled
        if (passiveTracking) {gameObject.transform.LookAt(player.transform.position);}

        //boss attack state machine
        switch(state) {
            default:
                state = State.IDLE;
                break;

            case State.IDLE:
                break;

            case State.CHARGE:
                if(Time.time <= chargeTime + boostDuration + chargeDuration) {
                    gameObject.transform.Translate(Vector3.forward * Time.deltaTime * (dist/chargeDuration));
                }
                else{
                    state = State.IDLE;
                    attacking = false;
                    gameObject.transform.SetPositionAndRotation(new Vector3(
                        gameObject.transform.position.x, 3, gameObject.transform.position.z),
                        gameObject.transform.rotation);
                    passiveTracking = true;
                }
                break;

            case State.BOOST_UP:
                if(Time.time <= chargeTime + boostDuration) {
                    gameObject.transform.Translate(Vector3.up * Time.deltaTime * (boostHeight/boostDuration), Space.World);
                }
                else {
                    state = State.CHARGE;
                    dist = Vector3.Distance(targetDest.position, gameObject.transform.position);
                }
                break;

            case State.SPIN:
                swordHitbox.SetActive(true);
                gameObject.transform.RotateAround(gameObject.transform.position, Vector3.up, spinSpeed * Time.deltaTime);
                if (Time.time >= spinTime + spinDuration) {
                    state = State.IDLE;
                    sword.transform.Rotate(-90, 0, 0);
                    attacking = false;
                    passiveTracking = true;
                    swordHitbox.SetActive(false);
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
        projectileOrigin.transform.LookAt(player.transform.position);

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
        if ((delegation.Other.gameObject.layer == 8 || delegation.Other.gameObject.layer == 11)) {
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
        else if(pick >= 75 && pick < 90) {
            Debug.Log(Vector3.Distance(gameObject.transform.position, player.transform.position));
            if(Vector3.Distance(gameObject.transform.position, player.transform.position) < 6.8f) {
                bossQ.Enqueue(Attack.SPIN);
            }
            else{
                bossQ.Enqueue(Attack.LASER);
            }
        
        }
        else {
            bossQ.Enqueue(Attack.LASER);
        }
    }

    void Gun(){
        if(fired < gunShots) {
            Rigidbody bulletInstance = Instantiate(bulletRB, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
            bulletInstance.velocity = projectileOrigin.transform.forward * bulletSpeed;
            attacking = true;
            fired += 1;
        }
        else {
            CancelInvoke("Gun");
            attacking = false;
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
        
    }

    void Spin(){
        Debug.Log("Spin!");
        passiveTracking = false;
        sword.transform.Rotate(90,0,0);
        spinTime = Time.time;
        state = State.SPIN;
        
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
