using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpread2D : MonoBehaviour
{
    //Spawns a fixed spead of bullets all at once

    public GameObject rotator;
    public Rigidbody2D bullet;
    public int bulletSpeed = 9;
    public int numOfBullets = 9;
    public float sphereRadius = 2.0f;
    public float spreadSize = 60.0f; //Size of the spread in degrees
    Transform placeBulletAimer;

    // Start is called before the first frame update
    void Start()
    {
        rotator.transform.position += new Vector3(-sphereRadius, 0, 0);

        float amountToRotate = spreadSize / (numOfBullets - 1);
        rotator.transform.Rotate(new Vector3(0, 0, -spreadSize / 2));

        //place first bullet
        Rigidbody2D firstBullet = Instantiate(bullet, gameObject.transform);
        firstBullet.transform.rotation = rotator.transform.rotation;
        firstBullet.transform.position += rotator.transform.right.normalized * sphereRadius;
        firstBullet.velocity = rotator.transform.right.normalized * bulletSpeed;

        for(int i = 0; i < numOfBullets - 1; i++)
        {
            Debug.Log("Spawned a bullet");
            rotator.transform.Rotate(new Vector3(0, 0, amountToRotate));

            Rigidbody2D newBullet = Instantiate(bullet, gameObject.transform);
            newBullet.transform.rotation = rotator.transform.rotation;
            newBullet.transform.position += rotator.transform.right.normalized * sphereRadius;
            newBullet.velocity = rotator.transform.right.normalized * bulletSpeed;
        }

        Destroy(gameObject, 10.0f);
    }
}
