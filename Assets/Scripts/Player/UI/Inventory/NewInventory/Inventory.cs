using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public GameObject slotContainer;
    public List<InventorySlot> inventorySlots;
    public List<InventoryPage> pages;
    public GameObject slotPrefab;
    public TextMeshProUGUI page;

    public RectTransform backgroundSize;
    public GridLayoutGroup layout;
    public int columnNumber;

    [SerializeField] private InputReader _inputReader = default;
    public bool isInventorySelected = false;
    InventoryManager manager;
    int selectedSlot = 0;

    int pageNumber = 0;

    public void Init(bool isItemBox)
    {
        manager = this.GetComponentInParent<InventoryManager>();
        foreach(Transform child in slotContainer.transform)
        {
            InventorySlot slot = child.gameObject.GetComponent<InventorySlot>();
            slot.Init(isItemBox);
            inventorySlots.Add(slot);
        }
        ResizeBackground();
        AddPage();
        AddPage();
        LoadPage(0);
    }

    public void AddSlots(int numberToAdd)
    {
        GameObject newSlot;

        for (int i = 0 ; i < numberToAdd ; i++)
        {
            newSlot = Instantiate(slotPrefab);
            newSlot.transform.parent = this.gameObject.transform;
        }
        ResizeBackground();
    }

    public void LoadPage(int newPage)
    {
        for(int i = 0; i < inventorySlots.Count ; i++)
        {
            pages[pageNumber].items[i] = inventorySlots[i].item;
            pages[pageNumber].values[i] = inventorySlots[i].numberValue;
        }
        pageNumber += newPage;
        if (pageNumber < 0)
            pageNumber = pages.Count - 1;
        else if (pageNumber > pages.Count - 1)
            pageNumber = 0;
        for (int i = 0; i < inventorySlots.Count ; i++)
        {
            inventorySlots[i].UpdateItem(pages[pageNumber].items[i], pages[pageNumber].values[i]);
        }
        if (page != null)
            page.text = pageNumber.ToString();
    }

    public void AddPage()
    {
        InventoryPage page = this.gameObject.AddComponent<InventoryPage>();
        page.Init(inventorySlots);
        pages.Add(page);
    }

    public void AddItem(ItemData item, int numberValue)
    {
        InventorySlot whereToAdd = null;

        foreach(InventorySlot slot in inventorySlots)
        {
            if (slot.item == null && whereToAdd == null)
            {
                whereToAdd = slot;
            }
            if (slot.item == item)
            {
                whereToAdd = null;
                if (slot.numberValue < slot.item.maxStack)
                {
                    slot.UpdateItem(item, numberValue);
                }
                return;
            }
        }
        if (whereToAdd != null)
        {
            whereToAdd.UpdateItem(item, numberValue);
        }
    }

    public void ResizeBackground()
    {
        float x = 0;
        float y = 0;
        int lineNumber = inventorySlots.Count / columnNumber;

        if (inventorySlots.Count % columnNumber != 0)
        {
            lineNumber += 1;
        }

        x = (layout.cellSize.x * columnNumber) + (layout.spacing.x * (columnNumber - 1)) + (layout.padding.left + layout.padding.right);
        y = (layout.cellSize.y * lineNumber) + (layout.spacing.y * (lineNumber - 1)) + (layout.padding.top + layout.padding.bottom);

        backgroundSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
        backgroundSize.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
    }

    public void SelectInventory(bool isSelected)
    {
        if (isSelected)
        {
            isInventorySelected = true;
            selectedSlot = 0;
        } else
        {
            isInventorySelected = false;
            selectedSlot = 0;
        }
    }

    public InventorySlot GetSelectedSlot()
    {
        if (isInventorySelected == true)
            return inventorySlots[selectedSlot];
        else
            return null;
    }

    void OnEnable()
    {
        _inputReader.navigateEvent += MoveHighlight;
    }

    void OnDisable()
    {
        _inputReader.navigateEvent -= MoveHighlight;
    }

    public void MoveHighlight(Vector2 button)
    {
        if (isInventorySelected)
        {
            if (button.y == -1)
            {
                selectedSlot += columnNumber;
                if (selectedSlot > inventorySlots.Count - 1)
                {
                    selectedSlot = selectedSlot % columnNumber;
                }
            } else if (button.y == 1)
            {
                selectedSlot += -columnNumber;
                if (selectedSlot < 0)
                {
                    selectedSlot = (inventorySlots.Count - 1) - (columnNumber - (selectedSlot + columnNumber + 1));
                }
            } else if (button.x == -1)
            {
                if (selectedSlot % columnNumber == 0)
                {
                    selectedSlot += columnNumber - 1;
                } else
                {
                    selectedSlot += -1;
                }
            } else if (button.x == 1)
            {
                if (selectedSlot % columnNumber == columnNumber - 1)
                {
                    selectedSlot -= columnNumber - 1;
                } else
                {
                    selectedSlot += 1;
                }
            }
            manager.SetHighlightPosition(inventorySlots[selectedSlot].GetComponent<RectTransform>().position);
            manager.ChangeDescription(inventorySlots[selectedSlot].item);
        }
    }
}
