using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeapon : MonoBehaviour
{
    private PlayerBehavior playerBehavior;
    private WeaponController weaponController; // Para manejar las armas recogidas
    private Weapon nearbyWeapon; // Para detectar armas en el suelo
   
    [SerializeField] private UIController uiController;

    [SerializeField] private float waitForAnim = 0.4f;
    [SerializeField] private GameObject[] weaponObjects;
    [SerializeField] private GameObject backWeaponObject;

    private Dictionary<WeaponType, GameObject> weaponDictionary = new Dictionary<WeaponType, GameObject>();
    private List<WeaponType> collectedWeapons = new List<WeaponType>();
    private int currentWeaponIndex = 0;

    private bool isNearWeapon = false;
    public bool hasPistol;
    private bool isSwitchingWeapon = false;
    public bool hasLargeWeapon = false;

    private WeaponType currentWeaponType;
    [SerializeField] private GameObject buttonIcon;

    private void Start()
    {
        buttonIcon.SetActive(false);
        playerBehavior = GetComponent<PlayerBehavior>();
        weaponController = GetComponentInChildren<WeaponController>(); // Maneja armas equipadas

        weaponDictionary.Clear();
        foreach (var weaponObj in weaponObjects)
        {
            if (weaponObj.TryGetComponent<Weapon>(out Weapon weaponComponent))
            {
                weaponDictionary[weaponComponent.weaponType] = weaponObj;
                weaponObj.SetActive(false);
            }
        }

        if (weaponDictionary.Count == 0)
        {
            Debug.LogError("No hay armas correctamente asignadas en el array weaponObjects.");
        }

        if (backWeaponObject != null)
        {
            backWeaponObject.SetActive(false);
        }
    }

    private void Update()
    {
        CollectWeapon();

        ChangeWeaponInput();
    }

    private void CollectWeapon()
    {
        // Recoger arma si estamos cerca y presionamos F
        if (isNearWeapon && Input.GetKeyDown(KeyCode.F) && nearbyWeapon != null)
        {
            WeaponType weaponType = nearbyWeapon.weaponType;

            if (!collectedWeapons.Contains(weaponType))
            {
                collectedWeapons.Add(weaponType);
            }

            playerBehavior.Animator.SetBool("pickUp", true);
            ActivateWeapon(weaponType);

            GameObject weaponObj = nearbyWeapon.gameObject;
            nearbyWeapon = null;
            isNearWeapon = false;

            Destroy(weaponObj); // Destruye después de limpiar la referencia
        }
    }

    private void ChangeWeaponInput()
    {
        if (collectedWeapons.Count > 1 && !isSwitchingWeapon)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                StartCoroutine(SwitchWeaponCoroutine(1));
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                StartCoroutine(SwitchWeaponCoroutine(-1));
            }
        }
    }

    private void ActivateWeapon(WeaponType type)
    {
        if (!collectedWeapons.Contains(type)) return;

        foreach (var weapon in weaponDictionary.Values)
        {
            weapon.SetActive(false);
        }

        if (weaponDictionary.TryGetValue(type, out GameObject weaponToActivate))
        {
            if (type == WeaponType.Short)
            {
                if (hasLargeWeapon && backWeaponObject != null)
                {
                    backWeaponObject.SetActive(true);
                }
                hasPistol = true;
            }
            else if (type == WeaponType.Long)
            {
                if (hasLargeWeapon && backWeaponObject != null)
                {
                    backWeaponObject.SetActive(false);
                }
                hasLargeWeapon = true;
            }

            weaponToActivate.SetActive(true);
            currentWeaponType = type;
            currentWeaponIndex = collectedWeapons.IndexOf(type);

            playerBehavior.Animator.SetBool("longWeapon", type == WeaponType.Long);
            playerBehavior.Animator.SetBool("shortWeapon", type == WeaponType.Short);

            if (weaponController != null)
            {
                weaponController.SetWeaponType(type);
            }
        }

        UpdateUI();
    }

    private IEnumerator SwitchWeaponCoroutine(int direction)
    {
        if (isSwitchingWeapon)
            yield break;

        isSwitchingWeapon = true;
        playerBehavior.Animator.SetBool("switchWeapon", true);
        yield return new WaitForSeconds(waitForAnim);

        currentWeaponIndex = (currentWeaponIndex + direction + collectedWeapons.Count) % collectedWeapons.Count;
        ActivateWeapon(collectedWeapons[currentWeaponIndex]);

        yield return new WaitForSeconds(0.1f);
        playerBehavior.Animator.SetBool("switchWeapon", false);
        isSwitchingWeapon = false;
    }

    private void UpdateUI()
    {
        uiController.UpdateWeaponUI(currentWeaponType, hasPistol, hasLargeWeapon);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Weapon>(out Weapon weapon))
        {
            buttonIcon.SetActive(true);
            nearbyWeapon = weapon;
            isNearWeapon = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == nearbyWeapon?.gameObject)
        {
            buttonIcon.SetActive(false);
            isNearWeapon = false;
            nearbyWeapon = null;
        }
    }
}
