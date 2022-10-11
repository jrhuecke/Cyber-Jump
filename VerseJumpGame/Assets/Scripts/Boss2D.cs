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

    
    private enum BehaviorResult { Success, Running, Failure};
    BehaviorResult lastFrameResult = BehaviorResult.Success;

    private enum Attack { None, SimpleBehavior};
    Attack currentBehavior = Attack.None;
    public float attackInterval = 2.0f;
    float doNotAttackTill;
    
    float waitTillTime = 0f; //Used by behaviors that wait until a certain amount of time has passed

    // Start is called before the first frame update
    void Start()
    {
        doNotAttackTill = Time.time + attackInterval;
    }

    void FixedUpdate()
    {
        //Lets assume the boss starts with no attack for now
        
        //Later will probably want a list of special behaviors that can override the main attack loop, like dialaogue or when the boss is defeated.
        //The attacks that are chosen randomly/in some sort of sequence will be put into a list to choose from.
        if(currentBehavior == Attack.None)
        {
            if (Time.time >= doNotAttackTill)
                currentBehavior = Attack.SimpleBehavior; //Replace w/ function that chooses an attack
        }
        if(currentBehavior != Attack.None)
        {
            switch (currentBehavior)
            {
                case Attack.SimpleBehavior:
                    lastFrameResult = simpleBehavior();
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

    //Later when the boss is defeated or switches phase - whatever the current behavior is must be interrupted.
}
