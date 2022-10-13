using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this script to any GameObject with a 2D collider that is a trigger to make it damage the player
public class DamagePlayer2D : MonoBehaviour
{
    public int DamageToPlayer;

    //Deal damage to player while overlapping them
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player2D playerScript = collision.gameObject.GetComponent<Player2D>();
        if(playerScript != null)
        {
            playerScript.TakeDamage(DamageToPlayer);
        }
    }
}
