using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    public int number;
    public TextMeshProUGUI textNumber;

    public int HEIGHT
    {
        get {
            if (!rotated)
                return itemData.height;
            return itemData.width;
        }
    }

    public int WIDTH
    {
        get {
            if (!rotated)
                return itemData.width;
            return itemData.height;
        }
    }

    public int onGridPositionX;
    public int onGridPositionY;

    public bool rotated = false;

    public void Set(ItemData itemData, int number)
    {
        GameObject child = this.gameObject.transform.GetChild(0).gameObject;
        this.itemData = itemData;
        this.number = number;
        this.textNumber = child.GetComponent<TextMeshProUGUI>();
        this.textNumber.text = this.number.ToString();

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = WIDTH * ItemGrid.tileSizeWidth;
        size.y = HEIGHT * ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void Set(ItemData itemData)
    {
        this.itemData = itemData;
        this.number = 0;
    }

    public void Rotate()
    {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0,0, rotated ? 90f : 0f);
    }
}
