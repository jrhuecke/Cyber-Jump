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
    
    private enum BehaviorResult { Success, Running, Failure};
    BehaviorResult lastFrameResult = BehaviorResult.Success;

    private enum Attack { None, SimpleBehavior};
    Attack currentBehavior = Attack.None;
    public float attackInterval = 2.0f;
    float doNotAttackTill;

    //"Blackboard" or variables that are used by behaviors
    public GameObject weaponRoot;
    public Player2D player;
    //When interrupting behaviors, we need to make sure that all of these get reset to their default values.
    float waitTillTime = 0f; //Used by behaviors that wait until a certain amount of time has passed
    Vector2 lockOnPosition = Vector2.zero;
    int repeatActionCounter = 0;
    int repeatThisMany = 0;
    bool swingingSword = false;

    // Start is called before the first frame update
    void Start()
    {
        animPlayer = gameObject.GetComponent<Animator>();
        doNotAttackTill = Time.time + attackInterval;
    }

    void FixedUpdate()
    {
        //Lets assume the boss starts with no attack for now
        
        //Later will probably want a list of special behaviors that can override the main attack loop, like dialaogue or when the boss is defeated.
        //The attacks that are chosen randomly/in some sort of sequence will be put into a list to choose from.
        //Special behavior may include simply moving to a new position, in which case they will not execute an attack until they have finished
        //moving, even if the attack interval is already finished (this does mean however they will attack the MOMENT they finish moving)

        if(currentBehavior == Attack.None)
        {
            if (Time.time >= doNotAttackTill)
                currentBehavior = Attack.SimpleBehavior; //Replace w/ function that chooses an attack
            else
            {
                //Look at player?
            }
        }
        if(currentBehavior != Attack.None)
        {
            switch (currentBehavior)
            {
                case Attack.SimpleBehavior:
                    lastFrameResult = swordSwipes();
                    break;
                default:
                    lastFrameResult = BehaviorResult.Failure;
                    Debug.LogError("Tried to call an attack that does not have a switch case!");
                    break;
            }
            if(lastFrameResult == BehaviorResult.Success || lastFrameResult == BehaviorResult.Failure)
            {
                currentBehavior = Attack.None;
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

    BehaviorResult swordSwipes()
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

        //First time call
        if(lastFrameResult != BehaviorResult.Running)
        {
            Debug.Log("Started sword slashes");
            lockOnPosition = player.transform.position;
            //Aim weaponRoot at the player (we might not even need a lock-on position...
            Vector2 aimDirection = new Vector2(player.transform.position.x - gameObject.transform.position.x, player.transform.position.y - gameObject.transform.position.y);
            float aimRotation = Vector2.Angle(Vector2.zero, aimDirection); //Returns 0 always?
            weaponRoot.transform.Rotate(new Vector3(0, 0, aimRotation - weaponRoot.transform.rotation.z));

            repeatActionCounter = 0;
            repeatThisMany = 3; //Choose random # of times to slash
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
            }
            else
            {
                animPlayer.Play("BossSwingSwordRightLeft", -1, 0);
            }
            //To create the bullets: spawn a group of bullets around the boss that are evenly spaced between each other
            swingingSword = true;
        }

        return BehaviorResult.Running;
    }

    //Called when sword slash anim finishes
    void finishedSwordSlash()
    {
        Debug.Log("Finished sword slash animation");
        repeatActionCounter += 1;
        swingingSword = false;
    }

    //Later when the boss is defeated or switches phase - whatever the current behavior is must be interrupted.
}
