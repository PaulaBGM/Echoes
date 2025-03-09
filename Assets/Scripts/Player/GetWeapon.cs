using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeapon : Player_Behavior
{
    [SerializeField] private GameObject[] weapon; // Array para las armas en la escena
    private bool isNearWeapon = false; // Variable para saber si est�s cerca de un arma
    private Weapon currentWeapon; // Referencia al arma que est� cerca del jugador

    protected override void Update()
    {
        base.Update();

        // Verifica si est�s cerca de un arma y presionas "F"
        if (isNearWeapon && Input.GetKeyDown(KeyCode.F))
        {
            if (currentWeapon != null)
            {
                // Verifica el tipo de arma y activa la correspondiente
                switch (currentWeapon.weaponType)
                {
                    case WeaponType.Long:
                        animator.SetBool("longWeapon", true);
                        animator.SetBool("shortWeapon", false);
                        ActivateWeapon(0); // Asumimos que el �ndice 0 es para el arma larga
                        break;

                    case WeaponType.Short:
                        animator.SetBool("shortWeapon", true);
                        animator.SetBool("longWeapon", false);
                        ActivateWeapon(1); // Asumimos que el �ndice 1 es para el arma corta
                        break;
                }

                // Despu�s de recoger el arma, destr�yela
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

        // Activa el arma correspondiente seg�n el �ndice
        if (index >= 0 && index < weapon.Length)
        {
            weapon[index].SetActive(true); // Activa el arma correcta
        }
    }

    // M�todo para manejar la entrada al �rea de un trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el objeto con el que colisiona tiene el componente IWeapons
        if (other.gameObject.GetComponent<IWeapons>() != null)
        {
            Debug.Log("PRESIONA F");

            // Obt�n el componente Weapon del objeto
            currentWeapon = other.gameObject.GetComponent<Weapon>();

            // Verifica si el objeto tiene el tag correcto y asigna la variable
            if (currentWeapon != null)
            {
                isNearWeapon = true;
            }
        }
    }
}
