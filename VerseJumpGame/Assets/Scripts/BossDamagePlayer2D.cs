using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamagePlayer2D : MonoBehaviour
{
    public int DamageToPlayer;

    //Deal damage to player while overlapping them
    private void OnTriggerStay2D(Collider2D collision)
    {
        Player2D playerScript = collision.gameObject.GetComponent<Player2D>();
        if(playerScript != null)
        {
            playerScript.TakeDamage(DamageToPlayer);
        }
    }
}
