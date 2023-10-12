using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot
{
    public ItemData item;
    public float percentage = 100;
    public int minQuantity = 1;
    public int maxQuantity = 1;

}
