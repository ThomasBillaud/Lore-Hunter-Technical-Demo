using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{
    public List<ItemData> items;
    public List<int> values;

    public void Init(List<InventorySlot> slots)
    {
        items = new List<ItemData>();
        values = new List<int>();

        foreach(InventorySlot slot in slots)
        {
            if (slot.item != null)
            {
                items.Add(slot.item);
                values.Add(slot.numberValue);
            } else
            {
                items.Add(null);
                values.Add(0);
            }
        }
    }
}
