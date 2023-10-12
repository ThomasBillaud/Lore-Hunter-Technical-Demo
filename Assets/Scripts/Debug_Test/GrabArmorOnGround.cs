using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabArmorOnGround : MonoBehaviour
{
    public ArmorItem armorToGive;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerArmorInventory armorInventory = other.gameObject.GetComponent<PlayerArmorInventory>();
            ArmorItem oldArmorPiece = armorInventory.armor[armorInventory.ChooseArmorItemPiece(armorToGive.type)];
            armorInventory.ChangeArmor(armorToGive);
            armorToGive = oldArmorPiece;
        }
    }

}
