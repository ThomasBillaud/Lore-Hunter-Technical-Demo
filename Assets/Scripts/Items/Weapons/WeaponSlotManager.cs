using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    public WeaponHolderSlot weaponSlot;
    public WeaponHolderSlot backSlot;

    public void LoadWeaponOnBack(WeaponItem weapon)
    {
        weaponSlot.UnloadWeapon();
        backSlot.LoadWeaponModel(weapon);
    }

    public void LoadWeaponOnHand(WeaponItem weapon)
    {
        backSlot.UnloadWeapon();
        weaponSlot.LoadWeaponModel(weapon);
    }

    public void ChangeWeapon(WeaponItem weapon)
    {
        backSlot.LoadWeaponModel(weapon);
    }
}
