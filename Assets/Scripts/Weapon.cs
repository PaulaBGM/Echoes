using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Long,
    Short
}

public class Weapon : MonoBehaviour, IWeapons
{
    public WeaponType weaponType; // Puede ser Long o Short

    public void DestroyWeapon()
    {
        Destroy(gameObject);
    }
}
