using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2D : MonoBehaviour
{
    /*
     General idea: Assuming has an attack selected, execute that function each frame
     The functions will return back running, success, or failure.
    If the function returns success, the boss will go into its interval behavior before
    choosing the next attack

    I guess making a legit behavior tree would involve creating a database and classes for each branch type,
    but I don't think we need to do that at this scope. Attacks are just gonna be random or a sequence.
     */
    Animator animPlayer;
    SpriteRenderer sprite;

    private enum BehaviorResult { Success, Running, Failure };
    BehaviorResult lastFrameResult = BehaviorResult.Success;

    private enum Behavior { None, SimpleBehavior, SwordSlashes };
    Behavior[] attackList = { Behavior.SwordSlashes }; //The behaviors that are attacks
    Behavior currentBehavior = Behavior.None;
    public float attackInterval = 2.0f;
    float doNotAttackTill;
    private System.Random rng = new System.Random();

    //"Blackboard" or variables that are used by behaviors
    public GameObject weaponRoot;
    public Player2D player;
    //When interrupting behaviors, we need to make sure that all of these get reset to their default values.
    float waitTillTime = 0f; //Used by behaviors that wait until a certain amount of time has passed
    int repeatActionCounter = 0;
    int repeatThisMany = 0;
    bool swingingSword = false;

    [Header("Sword Slash")]
    public BulletSpread2D bulletSpread;
    public float bulletSpreadSpeed = 6.0f;
    public int bulletSpeadCountOdd = 9;
    public float spreadSphereRadius = 2.0f;
    public float spreadSize = 120.0f;

    // Start is called before the first frame update
    void Start()
    {
        animPlayer = gameObject.GetComponent<Animator>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        doNotAttackTill = Time.time + attackInterval;
    }

    void FixedUpdate()
    {
        //Lets assume the boss starts with no attack for now
        
        //Later will probably want a list of special behaviors that can override the main attack loop, like dialaogue or when the boss is defeated.
        //The attacks that are chosen randomly/in some sort of sequence will be put into a list to choose from.
        //Special behavior may include simply moving to a new position, in which case they will not execute an attack until they have finished
        //moving, even if the attack interval is already finished (this does mean however they will attack the MOMENT they finish moving)

        if(currentBehavior == Behavior.None)
        {
            if (Time.time >= doNotAttackTill)
                currentBehavior = chooseAttack(); //Replace w/ function that chooses an attack
            else
            {
                //Look at player? (Only if not executing a behavior)
                if(player.transform.position.x > gameObject.transform.position.x)
                {
                    sprite.flipX = false;
                }
                else
                {
                    sprite.flipX = true;
                }
            }
        }
        if(currentBehavior != Behavior.None)
        {
            switch (currentBehavior)
            {
                case Behavior.SimpleBehavior:
                    lastFrameResult = simpleBehavior();
                    break;
                case Behavior.SwordSlashes:
                    lastFrameResult = swordSwipes(2, 6);
                    break;
                default:
                    lastFrameResult = BehaviorResult.Failure;
                    Debug.LogError("Tried to call an attack that does not have a switch case!");
                    break;
            }
            if(lastFrameResult == BehaviorResult.Success || lastFrameResult == BehaviorResult.Failure)
            {
                currentBehavior = Behavior.None;
                doNotAttackTill = Time.time + attackInterval;
            }
        }
    }

    //Example behavior that simply waits 3 seconds then finishes.
    BehaviorResult simpleBehavior()
    {
        //First frame that this behavior is called
        if(lastFrameResult != BehaviorResult.Running)
        {
            Debug.Log("Starting simple behavior");
            waitTillTime = Time.time + 3.0f;
            
        }

        if(Time.time >= waitTillTime)
        {
            Debug.Log("Finished simple behavior");
            return BehaviorResult.Success;
        }
        return BehaviorResult.Running;
    }

    /*Randomly choose from the list of attacks. May want to modify later to give certain attacks
     higher chances or whatever*/
    Behavior chooseAttack()
    {
        return attackList[rng.Next(0, attackList.Length)];
    }

    BehaviorResult swordSwipes(int min, int max)
    {
        /*
         * Start:
         *  Boss locks onto player's current position
         *  Boss chooses a random # of times to slash in that direction
         * Execution:
         *  Boss swings their sword the # of times they chose at the start, alternating the direction with each swing (also affecting the placement of the bullet spread)
         *  When finsihed swinging
         *      return sword to starting position
         *      return SUCCESS
         */
        //PROBLEM: The alternative slash doesn't space the bullets evenly between the bullets from the first slash, but I'll fix this later
        //I'd have to move the first bullet by a certain amount then double up the amount on the following bullets
        //Would prob be best to just create another bullet spread object

        //First time call
        if(lastFrameResult != BehaviorResult.Running)
        {
            Debug.Log("Started sword slashes");
            weaponRoot.transform.right = player.transform.position - gameObject.transform.position;

            repeatActionCounter = 0;
            repeatThisMany = Mathf.FloorToInt(rng.Next(min, max));
        }

        if(repeatActionCounter >= repeatThisMany)
        {
            Debug.Log("Finished sword slashes");
            return BehaviorResult.Success;
        }
        if (!swingingSword)
        {
            if (repeatActionCounter % 2 == 0) //Counter is even (will start here b/c counter starts at 0)
            {
                animPlayer.Play("BossSwingSwordLeftRight", -1, 0);
                BulletSpread2D bulletSpreadOdd = Instantiate(bulletSpread, weaponRoot.transform);
                bulletSpreadOdd.bulletSpeed = bulletSpreadSpeed;
                bulletSpreadOdd.numOfBullets = bulletSpeadCountOdd;
                bulletSpreadOdd.sphereRadius = spreadSphereRadius;
                bulletSpreadOdd.spreadSize = spreadSize;
            }
            else
            {
                animPlayer.Play("BossSwingSwordRightLeft", -1, 0);
                BulletSpread2D bulletSpreadEven = Instantiate(bulletSpread, weaponRoot.transform);
                bulletSpreadEven.bulletSpeed = bulletSpreadSpeed;
                bulletSpreadEven.numOfBullets = bulletSpeadCountOdd - 1;
                bulletSpreadEven.sphereRadius = spreadSphereRadius;
                bulletSpreadEven.spreadSize = spreadSize;
            }
            //To create the bullets: spawn a group of bullets around the boss that are evenly spaced between each other
            swingingSword = true;
        }

        return BehaviorResult.Running;
    }

    //Called when sword slash anim finishes
    void finishedSwordSlash()
    {
        //Debug.Log("Finished sword slash animation");
        repeatActionCounter += 1;
        swingingSword = false;
    }

    //Later when the boss is defeated or switches phase - whatever the current behavior is must be interrupted.

    public void TakeDamage(int damage)
    {
        Debug.Log("Boss took " + damage + " damage!");
    }
}
