using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpread2D : MonoBehaviour
{
    //Spawns a fixed spead of bullets all at once

    public Rigidbody2D bullet;
    public int numOfBullets = 9;
    public float sphereRadius = 2.0f;
    public float spreadSize = 60.0f; //Size of the spread in degrees
    Transform placeBulletAimer;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position += new Vector3(-sphereRadius, 0, 0);

        float amountToRotate = spreadSize / (numOfBullets - 1);
        gameObject.transform.Rotate(new Vector3(0, 0, -spreadSize / 2));

        //place first bullet
        Rigidbody2D firstBullet = Instantiate(bullet, gameObject.transform);
        firstBullet.transform.position += gameObject.transform.right.normalized * sphereRadius;

        //for num of bullets - 1
            //Rotate the placeBulletAimer
            //Place bullet at placeBulletAimer - (sphereRadius, 0, 0)
    }
}
