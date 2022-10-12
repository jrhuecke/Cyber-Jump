using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE: Used by both player and boss bullets
public class Bullet2D : MonoBehaviour
{
    public float lifetime = 5.0f;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject, 0);
    }
}
