using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponInventory : MonoBehaviour
{
    WeaponSlotManager weaponSlotManager;
    WeaponStatsApply applyStats;
    public WeaponItem weapon;

    private void Awake() {
        weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        applyStats = ScriptableObject.CreateInstance<WeaponStatsApply>();
        applyStats.Apply(this.gameObject, weapon);
    }

    private void Start() {
        SheathWeapon();
    }

    public void DrawWeapon()
    {
        weaponSlotManager.LoadWeaponOnHand(weapon);
    }

    public void SheathWeapon()
    {
        weaponSlotManager.LoadWeaponOnBack(weapon);
    }

    public void ChangeWeapon(WeaponItem newWeapon)
    {
        applyStats.Remove(this.gameObject, weapon);
        applyStats.Apply(this.gameObject, newWeapon);
        weapon = newWeapon;
        weaponSlotManager.LoadWeaponOnBack(weapon);
    }

    public GameObject GetWeaponModel()
    {
        return weaponSlotManager.weaponSlot.currentWeaponModel;
    }
}
