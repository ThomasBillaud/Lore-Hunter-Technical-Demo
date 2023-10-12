using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ArmorItem : ScriptableObject
{
    public enum armorType
    {
        Helmet,
        Torso,
        Gauntlets,
        Belt,
        Legs
    }

    public string itemName;
    public armorType type;
    public Sprite itemIcon;
    public GameObject modelPrefab;
    public int defense;
    public List<ASkill> skills;
}
