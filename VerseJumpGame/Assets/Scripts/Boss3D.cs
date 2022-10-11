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
    [SerializeField] int gunShots = 3; //# of machine gun shots
    [SerializeField] float bulletSpeed = 30f; //machine gun bullet speed
    [SerializeField] float shotInterval = 0.3f;
    private int fired; //track bullets fired and stop if it is >= gunshots

    private System.Random rng = new System.Random();
    private enum Attack{Gun, Charge, TurnAround, Missiles, Laser};
    private Queue<Attack> bossQ;
    [SerializeField] float attackRate = 3.0f; //at minimum 3 sec delay between attacks
    private float nextAttack; //used to calculate when boss can begin next attack
    private bool attacking = false; //is the boss currently in an attack animation

    private bool passiveTracking = true; //boss constantly turns to look at player if true.
    private bool isSpinning = false;
    private float spinTime = 0f;
    [SerializeField] float spinDuration = 1f;
    private float spinSpeed;

    private bool isCharging = false;
    private bool boostingUp = false;
    private float chargeTime = 0f;
    private Transform targetDest;
    private float dist;
    [SerializeField] float boostHeight = 3f;
    [SerializeField] float boostDuration = 0.5f;
    [SerializeField] float chargeDuration = 2f;

    public GameObject bulletPrefab;
    private Collider bulletColl;
    private Rigidbody bulletRB;
    public GameObject player;
    public GameObject projectileOrigin;
    public GameObject sword;

    // Start is called before the first frame update
    void Start()
    {
        bossQ = new Queue<Attack>();
        fired = 0;

        bulletRB = bulletPrefab.GetComponent<Rigidbody>();
        bulletColl = bulletPrefab.GetComponent<Collider>();

        Physics.IgnoreCollision(bulletColl, gameObject.GetComponent<Collider>());
        spinSpeed = (360/spinDuration);
        Debug.Log(spinSpeed);

    }

    // Update is called once per frame
    void Update()
    {
        //boss looking at player when enabled
        if (passiveTracking) {gameObject.transform.LookAt(player.transform.position);}

        //boss is spinning for turn attack when enabled
        if(isSpinning) {
            gameObject.transform.RotateAround(gameObject.transform.position, Vector3.up, spinSpeed * Time.deltaTime);
            if (Time.time >= spinTime + spinDuration) {
                isSpinning = false;
                sword.transform.Rotate(-90, 0, 0);
                attacking = false;
                passiveTracking = true;
            }
        }

        if(boostingUp) {
            if(Time.time <= chargeTime + boostDuration) {
                gameObject.transform.Translate(Vector3.up * Time.deltaTime * (boostHeight/boostDuration), Space.World);
            }
            else {
                boostingUp = false;
                isCharging = true;
                dist = Vector3.Distance(targetDest.position, gameObject.transform.position);
            }

        }
        if(isCharging) {
            if(Time.time <= chargeTime + boostDuration + chargeDuration) {
                gameObject.transform.Translate(Vector3.forward * Time.deltaTime * (dist/chargeDuration));
            }
            else{
                isCharging = false;
                attacking = false;
                gameObject.transform.SetPositionAndRotation(new Vector3(
                    gameObject.transform.position.x, 3, gameObject.transform.position.z),
                    gameObject.transform.rotation);
                passiveTracking = true;
            }
        }

        //tracking projectile origin to aim at player
        projectileOrigin.transform.LookAt(player.transform.position);

        //execute the next attack in the queue
        if(!attacking) {
            if (bossQ.Count > 0) {
                attacking = true;
                nextAttack = Time.time + attackRate;
                Attack curr = bossQ.Dequeue();
                switch(curr) {
                    case Attack.Gun:
                        Debug.Log("Gun!");
                        fired = 0;
                        InvokeRepeating("Gun", 0, shotInterval);
                        break;
                    case Attack.Charge:
                        Charge();
                        break;
                    case Attack.TurnAround:
                        TurnAround();
                        break;
                    case Attack.Missiles:
                        Missiles();
                        break;
                    case Attack.Laser:
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

    void TakeDamage() {
       Debug.Log("Boss took damage"); 
    }

    //attack selection ai
    void SelectAttack() {
        int pick = rng.Next(1,100);

        if(pick < 55) {
            Debug.Log(Vector3.Distance(gameObject.transform.position, player.transform.position));
            if(Vector3.Distance(gameObject.transform.position, player.transform.position) < 6.8f) {
                bossQ.Enqueue(Attack.TurnAround);
            }
            else{
                bossQ.Enqueue(Attack.Gun);
            }
        }
        else if(pick >= 55 && pick < 75) {
            bossQ.Enqueue(Attack.Charge);
            bossQ.Enqueue(Attack.TurnAround);
        }
        else if(pick >= 75 && pick < 90) {
            Debug.Log(Vector3.Distance(gameObject.transform.position, player.transform.position));
            if(Vector3.Distance(gameObject.transform.position, player.transform.position) < 6.8f) {
                bossQ.Enqueue(Attack.TurnAround);
            }
            else{
                bossQ.Enqueue(Attack.Laser);
            }
        
        }
        else {
            bossQ.Enqueue(Attack.Missiles);
        }

        //old 1-7 rng 
       /* switch(pick){
            case 1: 
                bossQ.Enqueue(Attack.Gun);
                return;
            case 2:
                bossQ.Enqueue(Attack.Charge);
                return;
            case 3: 
                bossQ.Enqueue(Attack.TurnAround);
                return;
            case 4:
                bossQ.Enqueue(Attack.Missiles);
                return;
            case 5: 
                bossQ.Enqueue(Attack.Laser);
                return;
            case 6:
                bossQ.Enqueue(Attack.Charge);
                bossQ.Enqueue(Attack.TurnAround);
                return;
            case 7:
                bossQ.Enqueue(Attack.Laser);
                bossQ.Enqueue(Attack.Gun);
                bossQ.Enqueue(Attack.Missiles);
                return;
        } */
    }

    void Gun(){
        if(fired < gunShots) {
            Rigidbody bulletInstance = Instantiate(bulletRB, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
            bulletInstance.velocity = projectileOrigin.transform.forward * bulletSpeed;
            Debug.Log("pew");
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
        boostingUp = true;
        chargeTime = Time.time;
        passiveTracking = false;
        targetDest = player.transform;
        gameObject.transform.LookAt(player.transform.position);
        
    }
    void TurnAround(){
        Debug.Log("TurnAround!");
        passiveTracking = false;
        sword.transform.Rotate(90,0,0);
        spinTime = Time.time;
        isSpinning = true;
        
    }
    void Missiles(){
        Debug.Log("Missiles!");
        attacking = false;
    }
    void Laser(){
        Debug.Log("Laser!");
        attacking = false;
    }

}
