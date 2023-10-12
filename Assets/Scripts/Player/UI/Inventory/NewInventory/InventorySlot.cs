using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item;
    public Image itemIcon;
    public TextMeshProUGUI numberText;
    public int numberValue = 0;
    public bool isEmpty = true;
    
    InventoryManager manager;
    bool isBoxSlot = false;

    public void Init(bool isItemBox)
    {
        UpdateItem(item, numberValue);
        manager = this.GetComponentInParent<InventoryManager>();
        isBoxSlot = isItemBox;
    }

    public void UpdateItem(ItemData newItem, int number = 0)
    {
        if (itemIcon != null && numberText != null)
        {
            var tempColor = itemIcon.color;
            numberValue = number;
            if (newItem == null || numberValue == 0)
            {
                tempColor.a = 0;
                itemIcon.color = tempColor;
                numberText.text = "";
                item = null;
            }
            else
            {
                tempColor.a = 255;
                itemIcon.color = tempColor;
                itemIcon.sprite = newItem.itemIcon;
                numberText.text = numberValue.ToString();
                item = newItem;
            }
        }
        if (manager != null && isBoxSlot == false)
            manager.UpdateHotbar();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        manager.ClickOnSlot(this, item);
        if (isEmpty == true)
            UpdateItem(null);
        manager.ChangeDescription(item);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Prout");
        manager.SetHighlightPosition(this.GetComponent<RectTransform>().position);
        manager.ChangeDescription(item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pipi");
        manager.SetHighlightPosition(new Vector3(10000, 10000, 0));
        manager.ChangeDescription(null);
    }
}
