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
    private enum Attack{Gun, Charge, TurnAround, Missles, Laser};
    private Queue<Attack> bossQ;
    private bool attacking = false; //will prevent the boss from overlapping attacks/used to space attacks out

    // Start is called before the first frame update
    void Start()
    {
        bossQ = new Queue<Attack>();

    }

    // Update is called once per frame
    void Update()
    {
        //execute the next attack in the queue
        if(!attacking) {
            if (bossQ.Count > 0) {
                attacking = true;
                Attack curr = bossQ.Dequeue;
                switch(curr) {
                    case Attack.Gun: Gun();
                    case Attack.Charge: Charge();
                    case Attack.TurnAround: TurnAround();
                    case Attack.Missles: Missiles();
                    case Attack.Laser: Laser();
                }
                attacking = false;
            }
        }

    }

    void Gun(){
        Debug.Log("Gun!");
    }
    void Charge(){
        Debug.Log("Charge!");
    }
    void TurnAround(){
        Debug.Log("TurnAround!");
    }
    void Missiles(){
        Debug.Log("Missiles!");
    }
    void Laser(){
        Debug.Log("Laser!");
    }

}
