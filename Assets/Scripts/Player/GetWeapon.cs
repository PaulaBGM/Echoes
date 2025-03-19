using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeapon : MonoBehaviour
{
    private PlayerBehavior playerBehavior;
    private Weapon currentWeapon; // Referencia al arma cercana

    [SerializeField] private float waitForAnim = 0.4f; // Tiempo de espera para la animación de cambio de arma
    [SerializeField] private GameObject[] weaponObjects; // Armas asignadas en el Inspector
    [SerializeField] private GameObject backWeaponObject; // Objeto que representa el arma en la espalda

    private Dictionary<WeaponType, GameObject> weaponDictionary = new Dictionary<WeaponType, GameObject>();
    private List<WeaponType> collectedWeapons = new List<WeaponType>(); // Lista de armas recogidas
    private int currentWeaponIndex = 0; // Índice del arma actualmente activa

    private bool isNearWeapon = false;
    public bool hasPistol; // Variable para saber si se tiene una pistola
    private bool isSwitchingWeapon = false; // Para evitar cambiar de arma rápidamente
    private bool hasLargeWeapon = false; // Para controlar si el jugador tiene el arma grande

    // Declaramos la variable currentWeaponType
    private WeaponType currentWeaponType; // Esto es necesario para hacer referencia al arma activa

    private void Start()
    {
        playerBehavior = GetComponent<PlayerBehavior>();

        // Llenar el diccionario dinámicamente sin depender del orden
        weaponDictionary.Clear();
        foreach (var weaponObj in weaponObjects)
        {
            if (weaponObj.TryGetComponent<Weapon>(out Weapon weaponComponent))
            {
                weaponDictionary[weaponComponent.weaponType] = weaponObj;
                weaponObj.SetActive(false); // Desactivar todas al inicio
            }
        }

        if (weaponDictionary.Count == 0)
        {
            Debug.LogError("No hay armas correctamente asignadas en el array weaponObjects.");
        }

        // Asegúrate de que el arma en la espalda esté desactivada al principio
        if (backWeaponObject != null)
        {
            backWeaponObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Recoger un arma cuando el jugador está cerca y presiona F
        if (isNearWeapon && Input.GetKeyDown(KeyCode.F))
        {
            if (currentWeapon != null)
            {
                if (!collectedWeapons.Contains(currentWeapon.weaponType))
                {
                    collectedWeapons.Add(currentWeapon.weaponType); // Agregar arma recogida
                }

                // Activar la animación de recoger arma
                playerBehavior.Animator.SetBool("pickUp", true);

                ActivateWeapon(currentWeapon.weaponType);
                currentWeapon.DestroyWeapon();
            }
        }

        // Cambio de arma con la rueda del mouse, pero solo si no estamos cambiando de arma
        if (collectedWeapons.Count > 1 && !isSwitchingWeapon) // Solo cambiar si hay al menos 2 armas y no estamos en medio de un cambio
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                StartCoroutine(SwitchWeaponCoroutine(1)); // Cambiar al siguiente arma
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                StartCoroutine(SwitchWeaponCoroutine(-1)); // Cambiar al arma anterior
            }
        }
    }

    private void ActivateWeapon(WeaponType type)
    {
        if (!collectedWeapons.Contains(type)) return; // No activar si no se recogió

        // Desactivar todas antes de activar la nueva
        foreach (var weapon in weaponDictionary.Values)
        {
            weapon.SetActive(false);
        }

        // Activar solo el arma seleccionada
        if (weaponDictionary.TryGetValue(type, out GameObject weaponToActivate))
        {
            // Si se selecciona un arma pequeña, guarda el arma grande en la espalda si ya se ha recogido el arma grande
            if (type == WeaponType.Short)
            {
                // Si el jugador ya tiene el arma grande, activamos el arma de la espalda
                if (hasLargeWeapon && backWeaponObject != null)
                {
                    backWeaponObject.SetActive(true); // Activar el arma en la espalda
                }
                hasPistol = true; // El jugador tiene ahora un arma pequeña (pistola)
            }
            else if (type == WeaponType.Long)
            {
                // Si se selecciona el arma grande, desactivar el arma de la espalda
                if (hasLargeWeapon && backWeaponObject != null)
                {
                    backWeaponObject.SetActive(false); // Desactivar el arma en la espalda
                }
                hasLargeWeapon = true; // El jugador tiene el arma grande
            }

            // Activar el arma seleccionada en las manos
            weaponToActivate.SetActive(true);
            currentWeaponType = type; // Establecer el arma activa

            currentWeaponIndex = collectedWeapons.IndexOf(type); // Actualizar el índice

            // Actualizar animaciones
            playerBehavior.Animator.SetBool("longWeapon", type == WeaponType.Long);
            playerBehavior.Animator.SetBool("shortWeapon", type == WeaponType.Short);
        }
    }

    // Coroutine para cambiar el arma
    private IEnumerator SwitchWeaponCoroutine(int direction)
    {
        // Si ya estamos cambiando de arma, no hacemos nada más
        if (isSwitchingWeapon)
            yield break;

        // Deshabilitar la capacidad de cambiar de arma hasta que la animación haya terminado
        isSwitchingWeapon = true;

        // Activar la animación de cambio de arma
        playerBehavior.Animator.SetBool("switchWeapon", true);

        // Esperar un tiempo para asegurar que la animación se vea
        yield return new WaitForSeconds(waitForAnim); // Ajusta el tiempo según la duración de la animación

        // Cambiar el arma después de la animación
        currentWeaponIndex = (currentWeaponIndex + direction + collectedWeapons.Count) % collectedWeapons.Count;
        ActivateWeapon(collectedWeapons[currentWeaponIndex]);

        // Esperar un poco más para que la animación de cambio de arma termine correctamente
        yield return new WaitForSeconds(0.1f); // Este pequeño delay da tiempo a que se inicie la nueva animación

        // Desactivar la animación de cambio de arma
        playerBehavior.Animator.SetBool("switchWeapon", false);

        // Volver a permitir cambios de arma
        isSwitchingWeapon = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Weapon>(out Weapon weapon))
        {
            Debug.Log("PRESIONA F para recoger el arma");
            currentWeapon = weapon;
            isNearWeapon = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentWeapon?.gameObject)
        {
            isNearWeapon = false;
            currentWeapon = null;
        }
    }
}