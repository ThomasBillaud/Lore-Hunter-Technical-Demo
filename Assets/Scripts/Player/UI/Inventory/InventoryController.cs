using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid { get => selectedItemGrid; set {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] private InputReader _inputReader = default;

    InventoryHighlight inventoryHighlight;
    InventoryItem itemToHighlight;
    Vector2Int oldPosition;

    public GameObject virtualMouse;
    public List<ItemData> items;
    public GameObject itemPrefab;
    public Transform canvasTransform;
    public GameObject inventoryObject;
    public RectTransform cursor;
    bool isLeftClick;
    bool isRightClick;
    Vector2 mousePosition;

    public delegate void InventoryChanges();
    public static event InventoryChanges inventoryChanges;

    private void Awake() {
        inventoryHighlight = GetComponent<InventoryHighlight>();
        selectedItemGrid.Init(selectedItemGrid.gridSizeWidth, selectedItemGrid.gridSizeHeight);
    }

    private void Update() {
        if (inventoryObject.activeSelf) {
            ItemIconDrag();
            if (!selectedItemGrid.isActive)
            {
                inventoryHighlight.Show(false);
                return;
            }
            HandleHighlight();
        }
    }

    public void OpenCloseInventory()
    {
        if (inventoryObject.activeSelf)
        {
            inventoryObject.SetActive(false);
            virtualMouse.SetActive(false);
        } else 
        {
            inventoryObject.SetActive(true);
            virtualMouse.SetActive(true);
        }
    }

    public void ItemIconDrag()
    {
        if (selectedItem != null)
            rectTransform.position = mousePosition;
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (positionOnGrid == new Vector2Int(-1, -1))
            return;
        oldPosition = positionOnGrid;
        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            if (itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            } else {
                inventoryHighlight.Show(false);
            }
        } else {
            inventoryHighlight.Show(selectedItemGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.WIDTH, selectedItem.HEIGHT));
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    public void ClickOnTile()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();
        if (tileGridPosition == new Vector2Int(-1, -1))
            return;

        if(selectedItem == null)
        {
            selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
            if (selectedItem != null)
                rectTransform = selectedItem.GetComponent<RectTransform>();
        } else {
            bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
            if (complete) {
                selectedItem = null;
                if (overlapItem != null) {
                    selectedItem = overlapItem;
                    overlapItem = null;
                    rectTransform = selectedItem.GetComponent<RectTransform>();
                    rectTransform.SetAsLastSibling();
                }
            }
        }
    }

    public Vector2Int GetTileGridPosition()
    {
        Vector2 position = mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }
        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    public bool AddToExistingStack(InventoryItem itemToInsert, int numberToAdd)
    {
        int rest = numberToAdd;
        bool isItemFound = false;
        foreach(InventoryItem item in selectedItemGrid.inventoryItemSlot)
        {
            if (item != null && itemToInsert.itemData.itemName == item.itemData.itemName)
            {
                isItemFound = true;
                if (item.number < item.itemData.maxStack || numberToAdd < 0)
                {
                    item.number += rest;
                    if (item.number > item.itemData.maxStack)
                    {
                        rest = item.number - item.itemData.maxStack;
                        item.number = item.itemData.maxStack;
                        item.textNumber.text = item.number.ToString();
                    } else {
                        rest = 0;
                        item.textNumber.text = item.number.ToString();
                        break;
                    }
                }
            }
        }
        return isItemFound;
    }

    public void InsertItem(InventoryItem itemToInsert, int numberToAdd)
    {
        bool isItemFound = AddToExistingStack(itemToInsert, numberToAdd);
        if (isItemFound) {
            inventoryChanges();
            return;
        }
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);
        
        if (posOnGrid == null)
            return;
        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y, numberToAdd);
        inventoryChanges();
    }

    public void InsertRandomItem()
    {
        if (!selectedItemGrid.isActive)
            return;
        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert, 1);
    }

    private void RotateItem()
    {
        if (selectedItem == null)
            return;
        selectedItem.Rotate();
    }

    void OnEnable()
    {
        _inputReader.inventoryEvent += OnInventory;
        _inputReader.leftClickInventoryEvent += OnLeftClickInventory;
        _inputReader.rightClickInventoryEvent += OnRightClickInventory;
        _inputReader.mousePosition += OnMousePosition;
    }

    void OnDisable()
    {
        _inputReader.inventoryEvent -= OnInventory;
        _inputReader.leftClickInventoryEvent -= OnLeftClickInventory;
        _inputReader.rightClickInventoryEvent -= OnRightClickInventory;
        _inputReader.mousePosition += OnMousePosition;
    }

    private void OnInventory()
    {
        OpenCloseInventory();
    }

    private void OnLeftClickInventory()
    {
        ClickOnTile();
    }

    private void OnRightClickInventory()
    {
        RotateItem();
    }

    private void OnMousePosition(Vector2 movement)
    {
        mousePosition = movement;
    }
}
