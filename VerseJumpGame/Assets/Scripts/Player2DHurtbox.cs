using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2DHurtbox : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("EnemyBullets"))
        {

        }

        /*If I wanted to access a damage variable on another game object, I would first have to getComponent<scriptname> of 
         a specific script that has the damage value. Since the things that damage the player probably won't have the same
        script names, trying to get their damage values from the player script is not practical*/
    }
}
