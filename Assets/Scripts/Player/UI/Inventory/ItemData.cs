using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public enum itemType
    {
        Consumable,
        Ammo,
        Material
    }

    public enum consumeAnim
    {
        None,
        Bandage
    }

    public string itemName = "Dummy";
    public int width = 1;
    public int height = 1;
    public int maxStack = 99;

    public Sprite itemIcon;
    public itemType typeOfItem;
    public bool isConsumable = false;
    public consumeAnim consumeAnimation = consumeAnim.None;
    public string descriptionKey;
}
