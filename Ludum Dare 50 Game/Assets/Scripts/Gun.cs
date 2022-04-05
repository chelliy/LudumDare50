using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bullet;
    public GameObject controller;
    public float bulletForce = 10f;
    public int damage = 1;
    public float horizontal;
    public float vertical;
    public AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
            audio.PlayOneShot(audio.clip);
            Debug.Log("audio");
        }

    }

    void Shoot()
    {
        GameObject b = Instantiate(bullet, firePoint.position, firePoint.rotation * bullet.transform.rotation);
        b.GetComponent<Bullet>().damage = damage;
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
        rb.AddForce((firePoint.up + firePoint.right) * bulletForce, ForceMode2D.Impulse);
    }
}
