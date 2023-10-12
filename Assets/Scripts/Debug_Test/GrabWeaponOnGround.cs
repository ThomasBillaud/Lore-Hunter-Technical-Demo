using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabWeaponOnGround : MonoBehaviour
{
    public WeaponItem weaponToGive;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerWeaponInventory weaponInventory = other.gameObject.GetComponent<PlayerWeaponInventory>();
            WeaponItem oldWeapon = weaponInventory.weapon;
            weaponInventory.ChangeWeapon(weaponToGive);
            weaponToGive = oldWeapon;
        }
    }
}
