using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBullet3D : MonoBehaviour
{
    public float lifetime = 5.0f;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.IgnoreCollision(GetComponent<Collider>(), GetComponent<Collider>());
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer != 9 && other.gameObject.layer != 7 && other.gameObject.layer != 10) {
            Destroy(gameObject, 0);
            Debug.Log("destroyed bullet layer " + other.gameObject.layer);
        }
    }
}
