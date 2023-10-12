using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmorInventory : MonoBehaviour
{
    ArmorStatsApply applyStats;
    [SerializeField] private ArmorSlot ArmorSlot;
    public ArmorItem[] armor = new ArmorItem[5];
    public ArmorItem helmet;
    public ArmorItem torso;
    public ArmorItem gauntlets;
    public ArmorItem belt;
    public ArmorItem boots;

    private void Awake()
    {
        applyStats = ScriptableObject.CreateInstance<ArmorStatsApply>();
        ApplyStatsForEveryArmorPiece();
    }

    private void Start()
    {
        foreach(ArmorItem armorPiece in armor)
        {
            ArmorSlot.LoadArmorPieceModel(armorPiece);
        }
    }

    public int ChooseArmorItemPiece(ArmorItem.armorType type)
    {
        switch(type)
        {
            case ArmorItem.armorType.Helmet:
                return 0;
            case ArmorItem.armorType.Torso:
                return 1;
            case ArmorItem.armorType.Gauntlets:
                return 2;
            case ArmorItem.armorType.Belt:
                return 3;
            case ArmorItem.armorType.Legs:
                return 4;
            default:
                return -1;
        }
    }

    public void ApplyStatsForEveryArmorPiece()
    {
        foreach(ArmorItem armorPiece in armor)
        {
            applyStats.Apply(this.gameObject, armorPiece);
        }
    }

    public void ChangeArmor(ArmorItem newArmorPiece)
    {
        Debug.Log("newArmorPiece = " + newArmorPiece.itemName);
        int piece = ChooseArmorItemPiece(newArmorPiece.type);
        Debug.Log("oldArmorPiece = " + armor[piece].itemName);
        applyStats.Remove(this.gameObject, armor[piece]);
        applyStats.Apply(this.gameObject, newArmorPiece);
        armor[piece] = newArmorPiece;
        Debug.Log("armor = " + armor[piece].itemName);
        Debug.Log("Legs = " + boots.itemName);
        ArmorSlot.LoadArmorPieceModel(armor[piece]);
    }
}