using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeapon : MonoBehaviour
{
    private PlayerLife playerHealth;
    private AimStateManager aimState;
    private Weapon currentWeapon; // Referencia al arma que está cerca del jugador

    [Header("Camera")]
    [SerializeField] private GameObject crosshairImage;
    [SerializeField] private GameObject normalCamera;
    [SerializeField] private GameObject pistolCamera;
    [SerializeField] private Transform camFollowPos_Pistol;

    [SerializeField] private GameObject[] weapon; // Array para las armas en la escena
    private bool isNearWeapon = false; // Variable para saber si estás cerca de un arma

    private void Start()
    {
        aimState = GetComponent<AimStateManager>();
        playerHealth = GetComponent<PlayerLife>();

        crosshairImage.SetActive(false);
        pistolCamera.SetActive(false);
        normalCamera.SetActive(true);
    }

    private void Update()
    {
        // Verifica si estás cerca de un arma y presionas "F"
        if (isNearWeapon && Input.GetKeyDown(KeyCode.F))
        {
            aimState.camFollowPos = camFollowPos_Pistol;
            crosshairImage.SetActive(true);
            pistolCamera.SetActive(true);
            normalCamera.SetActive(false);

            if (currentWeapon != null)
            {
                // Verifica el tipo de arma y activa la correspondiente
                switch (currentWeapon.weaponType)
                {
                    case WeaponType.Long:
                        playerHealth.Animator.SetBool("longWeapon", true);
                        playerHealth.Animator.SetBool("shortWeapon", false);
                        ActivateWeapon(0); // Asumimos que el índice 0 es para el arma larga
                        break;

                    case WeaponType.Short:
                        playerHealth.Animator.SetBool("shortWeapon", true);
                        playerHealth.Animator.SetBool("longWeapon", false);
                        ActivateWeapon(1); // Asumimos que el índice 1 es para el arma corta
                        break;
                }

                // Después de recoger el arma, destrúyela
                currentWeapon.DestroyWeapon();
            }
        }
    }

    private void ActivateWeapon(int index)
    {
        // Desactiva todas las armas en el array
        for (int i = 0; i < weapon.Length; i++)
        {
            weapon[i].SetActive(false); // Desactiva todas las armas
        }

        // Activa el arma correspondiente según el índice
        if (index >= 0 && index < weapon.Length)
        {
            weapon[index].SetActive(true); // Activa el arma correcta
        }
    }

    // Método para manejar la entrada al área de un trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto con el que colisiona tiene el componente IWeapons
        if (other.gameObject.GetComponent<IWeapons>() != null)
        {
            Debug.Log("PRESIONA F");

            // Obtén el componente Weapon del objeto
            currentWeapon = other.gameObject.GetComponent<Weapon>();

            // Verifica si el objeto tiene el tag correcto y asigna la variable
            if (currentWeapon != null)
            {
                isNearWeapon = true;
            }
        }
    }
}
