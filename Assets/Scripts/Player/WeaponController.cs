using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private AimStateManager aimState;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootSpawn;
    [SerializeField] private float fireRate;
    private float lastShootTime = 0f;

    [Header("Camera")]
    [SerializeField] private GameObject crosshairImage;
    [SerializeField] private GameObject normalCamera;
    [SerializeField] private GameObject pistolCamera;
    [SerializeField] private Transform camFollowPos_Pistol;
    [SerializeField] private Transform camFollowPos_Normal;

    public bool isShooting = false;
    public bool isAiming = false;

    public enum ShootMode { Single, Auto }
    public ShootMode currentShootMode = ShootMode.Auto; // Puedes cambiar a Single si lo deseas

    private Coroutine shootingCoroutine = null;

    void Update()
    {
        Debug.Log("TIEMPO DISAPRO: " + lastShootTime);

        // *** DISPARO ***
        if (Input.GetMouseButton(0))
        {
            isShooting = true;

            if (currentShootMode == ShootMode.Single)
            {
                Shoot();
            }
            else if (currentShootMode == ShootMode.Auto && shootingCoroutine == null)
            {
                shootingCoroutine = StartCoroutine(FireCoroutine());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
        }

        // *** APUNTADO ***
        if (Input.GetMouseButton(1)) // Se mantiene mientras el botón derecho esté presionado
        {
            isAiming = true;
            aimState.camFollowPos = camFollowPos_Pistol;
            crosshairImage.SetActive(true);
            pistolCamera.SetActive(true);
            normalCamera.SetActive(false);
        }
        else // Cuando se suelta, vuelve a la cámara normal
        {
            isAiming = false;
            aimState.camFollowPos = camFollowPos_Normal;
            crosshairImage.SetActive(false);
            pistolCamera.SetActive(false);
            normalCamera.SetActive(true);
        }

        // Asegurar que la posición de disparo siga la dirección de la mira
        AlignShootSpawnWithCamera();
    }

    void AlignShootSpawnWithCamera()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point; // Punto donde impacta el raycast
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 1000f; // Punto lejano si no impacta
        }

        Vector3 shootDirection = (targetPoint - shootSpawn.position).normalized;
        shootSpawn.rotation = Quaternion.LookRotation(shootDirection);
    }

    void Shoot()
    {
        if (Time.time - lastShootTime >= fireRate)
        {
            InstantiateBullet();
            lastShootTime = Time.time;
            isShooting = false;
        }
    }

    IEnumerator FireCoroutine()
    {
        while (isShooting)
        {
            Shoot();
            yield return new WaitForSeconds(fireRate);
            isShooting = false;
        }
    }

    void InstantiateBullet()
    {
        Instantiate(bulletPrefab, shootSpawn.position, shootSpawn.rotation);
    }
}
