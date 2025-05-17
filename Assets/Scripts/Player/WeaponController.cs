using System.Collections;
using UnityEngine;

public class WeaponController : Weapon
{
    [SerializeField] private AimStateManager aimState;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject particles;
    [SerializeField] private Transform shootSpawn;
    [SerializeField] private float rifleFireRate = 0.2f; // Tiempo base entre disparos
    [SerializeField] private float pistolFireRate = 0.5f; // La pistola tarda el doble
    private float lastShootTime = 0f;

    [Header("Camera")]
    [SerializeField] private GameObject crosshairImage;
    [SerializeField] private GameObject normalCamera;
    [SerializeField] private GameObject pistolCamera;
    [SerializeField] private Transform camFollowPos_Pistol;
    [SerializeField] private Transform camFollowPos_Normal;

    public bool isShooting = false;
    public bool isAiming = false;
    private AmmunitionManager ammoManager;
    private Coroutine shootingCoroutine;

    public WeaponType currentWeapon = WeaponType.Long; // Tipo de arma actual

    private void Start()
    {
        ammoManager = GetComponentInParent<AmmunitionManager>();
    }

    void Update()
    {
        HandleShooting();
        HandleAiming();
        AlignShootSpawnWithCamera();
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (shootingCoroutine == null)
                shootingCoroutine = StartCoroutine(AutoFireCoroutine());
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                shootingCoroutine = null;
            }
        }
    }

    IEnumerator AutoFireCoroutine()
    {
        while (true)
        {
            TryShoot();

            float fireRate = currentWeapon == WeaponType.Short
                ? pistolFireRate
                : rifleFireRate;

            yield return new WaitForSeconds(fireRate);
        }
    }

    void HandleAiming()
    {
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            aimState.camFollowPos = camFollowPos_Pistol;
            aimState.ShootCamera();
            crosshairImage.SetActive(true);
            pistolCamera.SetActive(true);
            normalCamera.SetActive(false);
        }
        else
        {
            isAiming = false;
            aimState.camFollowPos = camFollowPos_Normal;
            aimState.NormalCamera();
            crosshairImage.SetActive(false);
            pistolCamera.SetActive(false);
            normalCamera.SetActive(true);
        }
    }

    void AlignShootSpawnWithCamera()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Vector3 targetPoint;
        int enemyLayer = LayerMask.NameToLayer("Enemy_Detector");
        int layerMask = ~(1 << enemyLayer);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            targetPoint = hit.point;
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 1000f;
            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.green);
        }

        Debug.DrawLine(shootSpawn.position, targetPoint, Color.blue);

        Vector3 shootDirection = (targetPoint - shootSpawn.position).normalized;
        shootSpawn.rotation = Quaternion.LookRotation(shootDirection);
    }

    void TryShoot()
    {
        if (ammoManager.UseAmmo(1)) // Asegúrate de que hay munición
        {
            InstantiateBullet();
            lastShootTime = Time.time; // Actualiza el tiempo del último disparo
        }
        else
        {
            Debug.Log("No hay munición");
        }
    }

    void InstantiateBullet()
    {
        Instantiate(bulletPrefab, shootSpawn.position, shootSpawn.rotation);
        GameObject particlesGO = Instantiate(particles, shootSpawn.position, shootSpawn.rotation);
        Destroy(particlesGO, 0.2f); // Ajusta el tiempo al que dure tu efecto
    }

    public void SetWeaponType(WeaponType weapon)
    {
        currentWeapon = weapon;
    }
}
