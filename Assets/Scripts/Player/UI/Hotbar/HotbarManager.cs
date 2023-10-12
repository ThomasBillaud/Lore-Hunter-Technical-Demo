using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HotbarManager : MonoBehaviour
{
    public Animator animator;
    public CharacterMovement player;
    public Inventory inventory;
    [SerializeField] private InputReader _inputReader = default;

    InventorySlot[] itemArray;

    InventorySlot currentItem;
    InventorySlot previousItem;
    InventorySlot nextItem;
    InventorySlot emptySlot;

    public Hotbar hotbarCurrentItem;
    public Hotbar hotbarPreviousItem;
    public Hotbar hotbarNextItem;

    List<InventorySlot> itemList;
    int listPosition = 0;

    bool hotbarPressed;
    bool isMovingLeft;
    bool isMovingRight;

    int isOpenHash;

    // Start is called before the first frame update
    public void Init()
    {
        isOpenHash = Animator.StringToHash("isOpen");
        itemList = new List<InventorySlot>();
        GameObject obj = new GameObject();
        emptySlot = obj.AddComponent<InventorySlot>();
        obj.transform.SetParent(this.gameObject.transform);
        itemList.Insert(0, emptySlot);
        SetItemList();
        SetPictures();
    }

    void SetPictures()
    {
        if (listPosition >= itemList.Count)
            listPosition = 0;
        if (itemList.Count != 0) {
            currentItem = itemList[listPosition];
            if (listPosition + 1 >= itemList.Count)
                nextItem = itemList[0];
            else
                nextItem = itemList[listPosition + 1];
            if (listPosition - 1 < 0)
                previousItem = itemList[itemList.Count - 1];
            else
                previousItem = itemList[listPosition - 1];
            
            SetHotbar(currentItem, hotbarCurrentItem);
            SetHotbar(previousItem, hotbarPreviousItem);
            SetHotbar(nextItem, hotbarNextItem);
        }
    }

    void SetHotbar(InventorySlot item, Hotbar slot)
    {
        if (item != null)
        {
            if (item.item != null && item.item.itemIcon != null)
                slot.image.sprite = item.item.itemIcon;
            else
                slot.image.sprite = null;
            
            if (item.item != null && item.numberValue != 0)
                slot.itemNumber.text = item.numberValue.ToString();
            else
                slot.itemNumber.text = "";

            if (item.item != null && item.item.itemName != "Dummy")
                slot.itemName.text = item.item.itemName;
            else
                slot.itemName.text = "";
        }
    }

    public void SetItemList()
    {
        itemArray = inventory.inventorySlots.ToArray();
        itemList = new List<InventorySlot>();
        itemList.Insert(0, emptySlot);
        foreach(InventorySlot item in itemArray)
        {
            if (item.item != null && item.item.isConsumable == true)
            {
                itemList.Add(item);
            }
        }
        //string allItems = "";
        //foreach(InventorySlot item in itemList)
        //{
        //    allItems = allItems + "," + item.item.itemName;
        //}
        //Debug.Log("List items = " + allItems);
        SetPictures();
    }

    public void UseItem()
    {
        if (currentItem.item != null && currentItem.numberValue > 0)
        {
            player.handleItemUsageAnimation(currentItem.item.consumeAnimation);
            currentItem.UpdateItem(currentItem.item, currentItem.numberValue -1);
            if (currentItem.numberValue == 0) {
                SetHotbar(itemList[0], hotbarCurrentItem);
            }
            SetItemList();
        }
    }

    void OnEnable()
    {
        InventoryController.inventoryChanges += SetItemList;
        _inputReader.openHotbarEvent += OnOpen;
        _inputReader.moveHotbarLeftEvent += OnMoveLeft;
        _inputReader.moveHotbarRightEvent += OnMoveRight;
        _inputReader.useItemEvent += OnUseItem;
    }

    void OnDisable()
    {
        InventoryController.inventoryChanges -= SetItemList;
        _inputReader.openHotbarEvent -= OnOpen;
        _inputReader.moveHotbarLeftEvent -= OnMoveLeft;
        _inputReader.moveHotbarRightEvent -= OnMoveRight;
        _inputReader.useItemEvent -= OnUseItem;
    }

    private void OnOpen(bool isOpen)
    {
        hotbarPressed = isOpen;
        if (hotbarPressed)
            animator.SetBool(isOpenHash, true);
        else
            animator.SetBool(isOpenHash, false);
    }

    private void OnMoveLeft()
    {
        if (listPosition - 1 < 0)
            listPosition = itemList.Count - 1;
        else
            listPosition = listPosition - 1;
        SetPictures();
    }

    private void OnMoveRight()
    {
        if (listPosition + 1 > itemList.Count - 1)
            listPosition = 0;
        else
            listPosition = listPosition + 1;
        SetPictures();
    }

    private void OnUseItem()
    {
        UseItem();
    }
}
