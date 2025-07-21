using System;
using UnityEngine;

namespace Assets.Scripts.Inventory
{
    public class Grid<T>
    {
        public T[,] GridArray { get; }
        public int TileSize { get; }
        public int GridWidth { get; }
        public int GridHeight { get; }
        
        public RectTransform GridUI { get; }

        /// <summary>
        /// Creates a grid of type T
        /// </summary>
        /// <param name="gridUI"> RectTransform of grid user interface </param>
        /// <param name="tileSize"> in pixels </param>
        /// <param name="width"> number of horizontal columns </param>
        /// <param name="height"> number of vertical columns </param>
        public Grid(RectTransform gridUI, int tileSize, int width, int height)
        {
            GridArray = new T[width, height];
            
            TileSize = tileSize;
            GridWidth = width * tileSize;
            GridHeight = height * tileSize;
            GridUI = gridUI;

            GridUI.sizeDelta = new Vector2(GridWidth, GridHeight);
            GridUI.anchoredPosition = new Vector2(-GridWidth, GridHeight);
        }

        public Vector2Int GetTilePosition(Vector2 mousePosition)
        {
            var uiGridPos = GridUI.position;

            var gridPosX = mousePosition.x - uiGridPos.x;
            var gridPosY = uiGridPos.y - mousePosition.y;

            var tilePosX = gridPosX / TileSize;
            var tilePosY = gridPosY / TileSize;

            return new Vector2Int((int)tilePosX, (int)tilePosY);
        }
        
        public bool IsPositionOnGridValid(Vector2Int position)
        {
            var width = GridWidth / TileSize;
            var height = GridHeight / TileSize;
            
            if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
                return false;

            return true;
        }

        public T GetPositionOfT(Vector2Int position) => GridArray[position.x, position.y];

        public T PickupT(Vector2Int position, Action<T> removeFromGrid)
        {
            var pickup = GridArray[position.x, position.y];

            if (pickup is null) return pickup;
            
            // Clear grid reference (T)   
            removeFromGrid(pickup);
            
            return pickup;
        }
    }
}
