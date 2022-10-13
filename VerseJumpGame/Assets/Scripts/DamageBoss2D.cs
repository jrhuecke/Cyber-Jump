using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attach this script to any GameObject with a 2D collider that is a trigger to make it damage the boss
public class DamageBoss2D : MonoBehaviour
{
    public int DamageToBoss;

    //Deal damage to Boss when they first overlap
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Boss2D bossScript = collision.gameObject.GetComponent<Boss2D>();
        if (bossScript != null)
        {
            bossScript.TakeDamage(DamageToBoss);
        }
    }
}
