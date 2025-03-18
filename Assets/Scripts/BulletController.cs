using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float lifeTime = 4f;

    private float time = 0f;

    void Start()
    {
        Debug.Log("Bala creada con dirección: " + transform.forward);
    }

    void Update()
    {
        // Mueve la bala hacia adelante
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);

        Debug.Log("Moviendo bala a: " + transform.position + " con dirección: " + transform.forward);

        // Control de tiempo de vida de la bala
        time += Time.deltaTime;
        if (time >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}