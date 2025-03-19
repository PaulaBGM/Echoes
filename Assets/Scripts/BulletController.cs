using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float bulletPower = 10f;  // Dale un valor mayor a 0
    [SerializeField] private float lifeTime = 4f;
    private float time = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletPower, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}