using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float bulletPower = 10f;  // Dale un valor mayor a 0
    [SerializeField] private float damage = 15f;
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        if(other.CompareTag("Enemy"))
        {
            var damageable = other.gameObject.GetComponentInParent<IDamagable>();
            damageable.ApplyDamage(damage);
            Debug.Log($"Enemy hit: {other.name}, IDamageable found: {damageable != null}");
        }
    }
}