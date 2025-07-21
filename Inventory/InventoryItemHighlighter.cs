using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class InventoryItemHighlighter : MonoBehaviour
    {
        [SerializeField] private RectTransform highlighter;

        public void Show(bool show)
        {
            highlighter.gameObject.SetActive(show);
        }
        
        public void SetSize(Item itemToHighlight)
        {
            var size = new Vector2
            {
                x = itemToHighlight.Width * InventoryGrid.k_TileSize,
                y = itemToHighlight.Height * InventoryGrid.k_TileSize,
            };

            highlighter.sizeDelta = size;
        }
        
        public void SetPosition(InventoryGrid grid, Item item)
        {
            var position = grid.CalculatePositionOnGrid(item, item.tilePositionX, item.tilePositionY);
            highlighter.localPosition = position;
        }

        public void SetPosition(InventoryGrid grid, Item item, in Vector2Int pos)
        {
            var position = grid.CalculatePositionOnGrid(item, pos.x, pos.y);
            highlighter.localPosition = position;
        }
        
        public void SetParent(InventoryGrid grid)
        {
            if (!grid) return;
            highlighter.SetParent(grid.GetComponent<RectTransform>());
        }
    }
}
