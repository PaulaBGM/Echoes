using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float bulletPower = 10f;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float lifeTime = 4f;

    private float time = 0f;
    private Vector3 lastPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.AddForce(transform.forward * bulletPower, ForceMode.Impulse);
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        // Control de tiempo de vida
        time += Time.deltaTime;
        if (time >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        // Raycast para detectar colisiones incluso a alta velocidad
        Vector3 currentPosition = transform.position;
        Vector3 direction = currentPosition - lastPosition;
        float distance = direction.magnitude;

        if (distance > 0f)
        {
            if (Physics.Raycast(lastPosition, direction.normalized, out RaycastHit hit, distance))
            {
                Debug.Log($"Raycast hit: {hit.collider.name}");
                HandleHit(hit.collider);
            }
        }

        lastPosition = currentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other);
    }

    private void HandleHit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var damageable = other.GetComponentInParent<IDamagable>();
            if (damageable != null)
            {
                damageable.ApplyDamage(damage);
                Debug.Log("Impacto enemigo");
            }
        }

        if (other.CompareTag("TargetFront"))
        {
            TargetController target = other.GetComponentInParent<TargetController>();
            if (target != null)
            {
                target.withMovement = false;
                target.ChangeColor();
                Debug.Log("Impacto diana frontal");
            }
        }

        // En cualquier caso (frontal, trasera o enemigo), la bala se destruye
        if (other.CompareTag("TargetBack") || other.CompareTag("TargetFront") || other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}