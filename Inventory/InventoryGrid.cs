using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Inventory
{
    public class InventoryGrid : MonoBehaviour
    {
        [SerializeField] private RectTransform inventoryUIGrid;
        [SerializeField] private int width; // gridWidth
        [SerializeField] private int height; // gridHeight

        public const int k_TileSize = 32; // TileSize in pixels

        public Grid<Item> itemGrid;

        // Instantiate new Grid
        private void Awake() => itemGrid = new Grid<Item>(inventoryUIGrid, k_TileSize, width, height);

        public bool PlaceItem(in Item item, in Vector2Int pos, ref Item overlapItem)
        {
            if (!BoundaryCheck(pos, item))
                return false;

            if (!OverlapCheck(pos, item.Width, item.Height, ref overlapItem))
            {
                overlapItem = null;
                return false;
            }

            if (overlapItem)
            {
                // If same, check if it can stack, overlap item = item in inventory
                if (overlapItem.itemId == item.itemId)
                {
                    var maxStack = overlapItem.GetItemData.maxStack;
                    var total = overlapItem.stackCount + item.stackCount;

                    if (total < maxStack || (total > maxStack && overlapItem.stackCount < maxStack))
                    {
                        Item inlineItem;
                        (overlapItem, inlineItem) = HandleStackingItems(overlapItem, item, total);

                        if (inlineItem)
                            return false;

                        if (overlapItem is null)
                            return true;
                    }
                }

                ClearGridReference(overlapItem);
            }

            PlaceItem(item, pos.x, pos.y);
            return true;
        }

        public void PlaceItem(in Item item, int posX, int posY)
        {
            var itemTransform = item.GetComponent<RectTransform>();
            var itemImage = item.GetComponent<Image>();
            itemImage.SetNativeSize();
            itemTransform.SetParent(inventoryUIGrid);

            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    itemGrid.GridArray[posX + x, posY + y] = item;
                }
            }

            item.tilePositionX = posX;
            item.tilePositionY = posY;

            var position = CalculatePositionOnGrid(item, posX, posY);

            itemTransform.localPosition = position;
        }

        public Vector3 CalculatePositionOnGrid(Item item, int posX, int posY)
        {
            var position = new Vector3
            {
                x = posX * k_TileSize + k_TileSize * item.Width / 2,
                y = -(posY * k_TileSize + k_TileSize * item.Height / 2)
            };

            return position;
        }

        // Removes item from grid
        public void ClearGridReference(Item item)
        {
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    itemGrid.GridArray[item.tilePositionX + x, item.tilePositionY + y] = null;
                }
            }
        }

        // Is item within the bounds of the item grid
        public bool BoundaryCheck(Vector2Int pos, Item item)
        {
            if (!itemGrid.IsPositionOnGridValid(pos)) return false;

            pos.x += item.Width - 1;
            pos.y += item.Height - 1;

            if (!itemGrid.IsPositionOnGridValid(pos)) return false;

            return true;
        }

        private bool OverlapCheck(in Vector2Int pos, int w, int h, ref Item overlapItem)
        {
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (itemGrid.GridArray[pos.x + x, pos.y + y] != null)
                    {
                        if (!overlapItem)
                        {
                            overlapItem = itemGrid.GridArray[pos.x + x, pos.y + y];
                        }
                        else if (overlapItem != itemGrid.GridArray[pos.x + x, pos.y + y])
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool CheckAvailableSpace(int posX, int posY, int w, int h)
        {
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    if (itemGrid.GridArray[posX + x, posY + y] != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Vector2Int? FindSpaceForItem(Item item)
        {
            var w = width - item.Width + 1;
            var h = height - item.Height + 1;

            // Hard random
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (CheckAvailableSpace(x, y, item.Width, item.Height))
                    {
                        return new Vector2Int(x, y);
                    }
                }
            }

            return null;
        }

        private (Item, Item) HandleStackingItems(Item overlap, Item toPlace, int totalStackCount)
        {
            // Placing item with max stacks on item
            if (toPlace.GetItemData.maxStack == toPlace.stackCount)
            {
                toPlace.UpdateStackCount(overlap.stackCount);
                overlap.UpdateStackCount(overlap.GetItemData.maxStack);
                return (null, toPlace);
            }

            // If total stacks is more than max stack of overlap item
            if (totalStackCount > overlap.GetItemData.maxStack)
            {
                overlap.UpdateStackCount(overlap.GetItemData.maxStack);
                var leftOverStacks = totalStackCount - overlap.GetItemData.maxStack;
                toPlace.UpdateStackCount(leftOverStacks);

                return (overlap, null);
            }

            // if total stacks is less than max stacks just add them to overlap item and remove item that is placing
            overlap.UpdateStackCount(totalStackCount);

            ClearGridReference(toPlace);
            toPlace.DestroyItem();
            return (null, null);
        }
    }
}