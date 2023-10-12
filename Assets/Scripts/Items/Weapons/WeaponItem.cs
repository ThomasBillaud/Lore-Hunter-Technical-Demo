using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class WeaponItem : ScriptableObject
{
    public enum weaponType
    {
        Martaxe,
        Crossbow
    }

    public string itemName;
    public bool isUnarmed;
    public weaponType type;
    public Sprite itemIcon;
    public GameObject modelPrefab;
    public int attack;
    public int critChance;
    public int defense;
}
