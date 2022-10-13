using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeTargetPos2D : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Boss2D bossAI = collision.gameObject.GetComponent<Boss2D>();
        if(bossAI != null)
        {
            bossAI.StopCharging(false);
        }
    }
}
