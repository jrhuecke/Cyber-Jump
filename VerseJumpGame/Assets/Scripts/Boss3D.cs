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
    [SerializeField] float bulletSpeed = 12f; //machine gun bullet speed
    [SerializeField] float shotInterval = 0.3f;
    private int fired; //track bullets fired and stop if it is >= gunshots

    private System.Random rng = new System.Random();
    private enum Attack{Gun, Charge, TurnAround, Missiles, Laser};
    private Queue<Attack> bossQ;
    [SerializeField] float attackRate = 3.0f; //at minimum 3 sec delay between attacks
    private float nextAttack; //used to calculate when boss can begin next attack
    private bool attacking = false; //is the boss currently in an attack animation

    public GameObject bulletPrefab;
    private Collider bulletColl;
    private Rigidbody bulletRB;
    public GameObject player;
    public GameObject projectileOrigin;

    // Start is called before the first frame update
    void Start()
    {
        bossQ = new Queue<Attack>();
        fired = 0;

        bulletRB = bulletPrefab.GetComponent<Rigidbody>();
        bulletColl = bulletPrefab.GetComponent<Collider>();

        Physics.IgnoreCollision(bulletColl, gameObject.GetComponent<Collider>());



    }

    // Update is called once per frame
    void Update()
    {
        //boss looking at player
        gameObject.transform.LookAt(player.transform.position);
        //tracking projectile origin to also look at player
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

        if(!attacking && Time.time > nextAttack) {
            SelectAttack();
        }
    }

    void TakeDamage() {
       Debug.Log("Boss took damage"); 
    }

    void SelectAttack() {
        int pick = rng.Next(1,7);

        switch(pick){
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
        } 
    }

    void Gun(){
        if(fired < gunShots) {
            Rigidbody bulletInstance = Instantiate(bulletRB, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
            bulletInstance.velocity = gameObject.transform.forward * bulletSpeed;
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
        attacking = false;
    }
    void TurnAround(){
        Debug.Log("TurnAround!");
        attacking = false;
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
