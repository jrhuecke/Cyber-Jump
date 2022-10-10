using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet3D : MonoBehaviour
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

    private void OnTriggerEnter(Collider collision)
    {
            Destroy(gameObject, 0);
    }
}
