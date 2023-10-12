using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    //description
    public Description itemDescription;

    //Move an item
    public GameObject itemPrefab;
    public GameObject selectedItem;
    public RectTransform rectTransform;
    public ItemData heldItem;
    int heldNumberValue;
    Vector3 mousePosition;
    bool isUsingController = false;

    //Load other pages 
    public GameObject boxInventory;
    public GameObject inventory;
    public GameObject description;
    public GameObject highlight;
    public HotbarManager hotbar;
    private Inventory baseInventory;
    private Inventory itemBox;

    //Controls
    [SerializeField] private InputReader _inputReader = default;


    // Start is called before the first frame update
    void Start()
    {
        itemBox = boxInventory.GetComponent<Inventory>();
        itemBox.Init(true);
        baseInventory = inventory.GetComponent<Inventory>();
        baseInventory.Init(false);
        hotbar.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedItem != null && rectTransform != null)
        {
            ItemIconDrag();
        }
    }

    public void ItemIconDrag()
    {
        if (selectedItem != null && isUsingController == false)
        {
            mousePosition = Mouse.current.position.ReadValue();
            mousePosition.z = Camera.main.nearClipPlane;
            rectTransform.position = mousePosition;
        } else if (selectedItem != null && isUsingController == true)
        {
            Vector3 highlightTransform = highlight.GetComponent<RectTransform>().position;
            rectTransform.position = new Vector3(highlightTransform.x + 25, highlightTransform.y + 25, highlightTransform.z);
        }
    }

    public void ClickOnSlot(InventorySlot slot, ItemData item)
    {
        if (selectedItem == null && item != null)
        {
            selectedItem = Instantiate(itemPrefab);
            selectedItem.GetComponent<Image>().sprite = item.itemIcon;
            selectedItem.GetComponentInChildren<TextMeshProUGUI>().text = slot.numberValue.ToString();
            rectTransform = selectedItem.GetComponent<RectTransform>();
            rectTransform.SetParent(this.transform);
            rectTransform.SetAsLastSibling();
            heldItem = item;
            heldNumberValue = slot.numberValue;
            slot.isEmpty = true;
            ItemIconDrag();
        } else if (selectedItem != null && item == null)
        {
            slot.UpdateItem(heldItem, heldNumberValue);
            slot.isEmpty = false;
            rectTransform = null;
            heldItem = null;
            heldNumberValue = 0;
            Destroy(selectedItem);
        } else if (selectedItem != null && item != null)
        {
            Debug.Log(heldItem.itemName + " && " + item.itemName);
            if (heldItem.itemName == item.itemName)
            {
                slot.UpdateItem(heldItem, slot.numberValue + heldNumberValue);
                slot.isEmpty = false;
                rectTransform = null;
                heldItem = null;
                heldNumberValue = 0;
                Destroy(selectedItem);
            } else
            {
                ItemData newSelectedItem = item;
                int newHeldValue = slot.numberValue;
                slot.UpdateItem(heldItem, heldNumberValue);
                selectedItem.GetComponent<Image>().sprite = newSelectedItem.itemIcon;
                selectedItem.GetComponentInChildren<TextMeshProUGUI>().text = newHeldValue.ToString();
                heldItem = item;
                heldNumberValue = newHeldValue;
                slot.isEmpty = false;
                ItemIconDrag();
            }
        }
    }

    public void SlotInteract()
    {
        InventorySlot selectedSlot = baseInventory.GetSelectedSlot();

        if (selectedSlot == null)
            selectedSlot = itemBox.GetSelectedSlot();
        MoveItemFromSlot(selectedSlot, selectedSlot.item);
        ChangeDescription(selectedSlot.item);
    }

    void MoveItemFromSlot(InventorySlot slot, ItemData item)
    {
        if (selectedItem == null && item != null)
        {
            isUsingController = true;
            selectedItem = Instantiate(itemPrefab);
            selectedItem.GetComponent<Image>().sprite = item.itemIcon;
            selectedItem.GetComponentInChildren<TextMeshProUGUI>().text = slot.numberValue.ToString();
            rectTransform = selectedItem.GetComponent<RectTransform>();
            rectTransform.SetParent(highlight.transform);
            rectTransform.SetAsLastSibling();
            heldItem = item;
            heldNumberValue = slot.numberValue;
            slot.isEmpty = true;
            slot.UpdateItem(null);
        } else if (selectedItem != null && item == null)
        {
            isUsingController = false;
            slot.UpdateItem(heldItem, heldNumberValue);
            slot.isEmpty = false;
            rectTransform = null;
            heldItem = null;
            heldNumberValue = 0;
            Destroy(selectedItem);
        } else if (selectedItem != null && item != null)
        {
            if (heldItem.itemName == item.itemName)
            {
                slot.UpdateItem(heldItem, slot.numberValue + heldNumberValue);
                slot.isEmpty = false;
                rectTransform = null;
                heldItem = null;
                heldNumberValue = 0;
                Destroy(selectedItem);
            } else
            {
                isUsingController = true;
                ItemData newSelectedItem = item;
                int newHeldValue = slot.numberValue;
                slot.UpdateItem(heldItem, heldNumberValue);
                selectedItem.GetComponent<Image>().sprite = newSelectedItem.itemIcon;
                selectedItem.GetComponentInChildren<TextMeshProUGUI>().text = newHeldValue.ToString();
                heldItem = item;
                heldNumberValue = newHeldValue;
                slot.isEmpty = false;
            }
        }
    }

    public void SetHighlightPosition(Vector3 newPosition)
    {
        highlight.GetComponent<RectTransform>().position = newPosition;
    }

    public void ChangeDescription(ItemData itemData)
    {
        itemDescription.ChangeDescription(itemData);
    }

    public void LoadPreviousPage()
    {
        itemBox.LoadPage(-1);
    }

    public void LoadNextPage()
    {
        itemBox.LoadPage(1);
    }

    public void UpdateHotbar()
    {
        hotbar.SetItemList();
    }

    void OnEnable()
    {
        _inputReader.inventoryEvent += OnInventory;
        _inputReader.switchToInventoryEvent += SwitchToInventory;
        _inputReader.switchToItemBoxEvent += SwitchToItemBox;
        _inputReader.leftClickInventoryEvent += SlotInteract;
    }

    void OnDisable()
    {
        _inputReader.inventoryEvent -= OnInventory;
        _inputReader.switchToInventoryEvent -= SwitchToInventory;
        _inputReader.switchToItemBoxEvent -= SwitchToItemBox;
        _inputReader.leftClickInventoryEvent -= SlotInteract;
    }

    public void OnInventory()
    {
        if (inventory.activeSelf)
        {
            inventory.SetActive(false);
            description.SetActive(false);
            boxInventory.SetActive(false);
            highlight.SetActive(false);
            _inputReader.DisableUIInput();
            _inputReader.IsInventoryOpenOrClosed(false);
        } else
        {
            inventory.SetActive(true);
            description.SetActive(true);
            highlight.SetActive(true);
            _inputReader.EnableUIInput();
            _inputReader.IsInventoryOpenOrClosed(true);
            baseInventory.SelectInventory(true);
            itemBox.SelectInventory(false);
            SetHighlightPosition(baseInventory.inventorySlots[0].GetComponent<RectTransform>().position);
            ChangeDescription(baseInventory.inventorySlots[0].item);
        }
    }

    public void OnItemBoxInventory()
    {
        if (boxInventory.activeSelf)
        {
            inventory.SetActive(false);
            description.SetActive(false);
            boxInventory.SetActive(false);
            highlight.SetActive(false);
            _inputReader.DisableUIInput();
            _inputReader.IsInventoryOpenOrClosed(false);
        } else
        {
            inventory.SetActive(true);
            description.SetActive(true);
            boxInventory.SetActive(true);
            highlight.SetActive(true);
            _inputReader.EnableUIInput();
            _inputReader.IsInventoryOpenOrClosed(true);
            baseInventory.SelectInventory(true);
            itemBox.SelectInventory(false);
            SetHighlightPosition(baseInventory.inventorySlots[0].GetComponent<RectTransform>().position);
            ChangeDescription(baseInventory.inventorySlots[0].item);
        }
    }

    public void SwitchToInventory()
    {
        if (boxInventory.activeInHierarchy)
        {
            baseInventory.SelectInventory(true);
            itemBox.SelectInventory(false);
            SetHighlightPosition(baseInventory.inventorySlots[0].GetComponent<RectTransform>().position);
            ChangeDescription(baseInventory.inventorySlots[0].item);
        }
    }

    public void SwitchToItemBox()
    {
        if (boxInventory.activeInHierarchy)
        {
            itemBox.SelectInventory(true);
            baseInventory.SelectInventory(false);
            SetHighlightPosition(itemBox.inventorySlots[0].GetComponent<RectTransform>().position);
            ChangeDescription(itemBox.inventorySlots[0].item);
        }
    }
}
