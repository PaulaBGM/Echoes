using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootSpawn;

    void Update()
    {
        Debug.DrawLine(shootSpawn.position, shootSpawn.position + shootSpawn.forward * 10f, Color.red);
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 10, Color.blue);

        RaycastHit cameraHit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit))
        {
            Vector3 shootDirection = cameraHit.point - shootSpawn.position;
            shootSpawn.rotation = Quaternion.LookRotation(shootDirection);
        }

        // Depurar la dirección del spawn
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Shooting();
        }
    }

    private void Shooting()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootSpawn.position, shootSpawn.rotation);
    }
}
