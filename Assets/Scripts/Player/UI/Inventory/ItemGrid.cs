using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;
    public InventoryItem[,] inventoryItemSlot;
    RectTransform rectTransform;
    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    public int gridSizeWidth = 20;
    public int gridSizeHeight = 10;

    public bool isActive = false;

    public void Init(int width, int height)
    {
        rectTransform = GetComponent<RectTransform>();
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    public Vector2Int GetTileGridPosition(Vector2 position)
    {
        positionOnTheGrid.x = position.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - position.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        if (tileGridPosition.x > inventoryItemSlot.GetLength(0) || tileGridPosition.x < 0 || tileGridPosition.y > inventoryItemSlot.GetLength(1) || tileGridPosition.y < 0)
            tileGridPosition = new Vector2Int(-1, -1);
        else
            Debug.Log("tileGridPosition = " + tileGridPosition);

        return tileGridPosition;
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (!BoundaryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT))
            return false;
        if (!OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem)) {
            overlapItem = null;
            return false;
        }
        if (overlapItem != null)
            CleanGridReference(overlapItem);

        PlaceItem(inventoryItem, posX, posY, inventoryItem.number);
        return true;
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY, int value)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }
        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        if (inventoryItem.itemData.maxStack < value)
        {
            inventoryItem.number = inventoryItem.itemData.maxStack;
            value -= inventoryItem.number;
        } else {
            inventoryItem.number = value;
            value = 0;
        }
        if (inventoryItem.textNumber != null)
            inventoryItem.textNumber.text = inventoryItem.number.ToString();

        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.HEIGHT / 2);
        return position;
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null)
            return null;

        CleanGridReference(toReturn);
        return toReturn;
    }

    public void CleanGridReference(InventoryItem toReturn)
    {
        for (int ix = 0; ix < toReturn.WIDTH; ix++)
        {
            for (int iy = 0; iy < toReturn.HEIGHT; iy++)
            {
                inventoryItemSlot[toReturn.onGridPositionX + ix, toReturn.onGridPositionY + iy] = null;
            }
        }
    }

    bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX+x, posY+y] != null)
                {
                    if (overlapItem == null)
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                } else if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                    return false;
            }
        }
        return true;
    }

    bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX+x, posY+y] != null)
                    return false;
            }
        }
        return true;
    }

    bool PositionCheck(int posX, int posY)
    {
        if (posX < 0 || posY < 0)
            return false;
        if (posX >= gridSizeWidth || posY >= gridSizeHeight)
            return false;
        return true;
    }

    public bool BoundaryCheck(int posX, int posY, int width, int height)
    {
        if (!PositionCheck(posX, posY))
            return false;
        
        posX += width-1;
        posY += height-1;
        
        if (!PositionCheck(posX, posY))
            return false;
        return true;
    }

    public InventoryItem GetItem(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < inventoryItemSlot.GetLength(0) && y < inventoryItemSlot.GetLength(1))
            return inventoryItemSlot[x, y];
        return null;
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT))
                    return new Vector2Int(x, y);
            }
        }
        return null;
    }
}
