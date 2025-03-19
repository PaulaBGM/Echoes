using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private AimStateManager aimState;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootSpawn;
    [SerializeField] private float fireRate = 0.1f; // Disparo cada 0.1 segundos
    private float lastShootTime = 0f;


    [Header("Camera")]
    [SerializeField] private GameObject crosshairImage;
    [SerializeField] private GameObject normalCamera;
    [SerializeField] private GameObject pistolCamera;
    [SerializeField] private Transform camFollowPos_Pistol;
    [SerializeField] private Transform camFollowPos_Normal;

    public bool isShooting = false;

    public enum ShootMode
    {
        Single,
        Auto
    }

    public ShootMode currentShootMode = ShootMode.Single;

    private void Start()
    {
        aimState.camFollowPos = camFollowPos_Normal;

        crosshairImage.SetActive(false);
        pistolCamera.SetActive(false);
        normalCamera.SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isShooting)
        {
            isShooting = true;

            Shoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }

        Debug.DrawLine(shootSpawn.position, shootSpawn.position + shootSpawn.forward * 10f, Color.red);
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * 10, Color.blue);

        RaycastHit cameraHit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out cameraHit))
        {
            Vector3 shootDirection = cameraHit.point - shootSpawn.position;
            shootSpawn.rotation = Quaternion.LookRotation(shootDirection);
        }

        if(Input.GetMouseButtonDown(1) && !isShooting)
        {
            aimState.camFollowPos = camFollowPos_Pistol;
            crosshairImage.SetActive(true);
            pistolCamera.SetActive(true);
            normalCamera.SetActive(false);
        }

       else if (Input.GetMouseButtonUp(1))
        {
            aimState.camFollowPos = camFollowPos_Normal;
            crosshairImage.SetActive(false);
            pistolCamera.SetActive(false);
            normalCamera.SetActive(true);
        }
    }

    private void Shoot()
    {
        if(isShooting)
        {
            if(Time.time - lastShootTime > fireRate)
            {
                switch (currentShootMode)
                {
                    case ShootMode.Single:
                        InstantiateBullet();
                        break;
                    case ShootMode.Auto:
                        StartCoroutine(FireCoroutine());
                        break;
                }
            }
        }
    }

    IEnumerator FireCoroutine()
    {
        while (isShooting)
        {
            InstantiateBullet();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void InstantiateBullet()
    {
        Instantiate(bulletPrefab, shootSpawn.position, shootSpawn.rotation);
    }
}
