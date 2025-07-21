using System.Collections.Generic;
using Assets.Scripts.Framework.Extensions;
using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Inventory
{
    [RequireComponent(typeof(InventoryGrid))]
    public class InventoryController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private InventoryGrid m_InventoryGrid;
        private Item m_CurrentItem;
        private Item m_OverlapItem;
        private Item m_ItemToHighlight;
        private RectTransform m_ItemTransform;
        private Vector2Int m_OldPosition;

        [SerializeField] private List<ItemData> itemsData;
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform canvasTransform;
        [SerializeField] private InventoryItemHighlighter itemHighlighter;

        public InventoryGrid InventoryGrid
        {
            get
            {
                if (!m_InventoryGrid)
                {
                    return GetComponent<InventoryGrid>();
                }
                else return m_InventoryGrid;
            }

            set
            {
                m_InventoryGrid = value;
                itemHighlighter.SetParent(value);
            }
        }

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!gameObject.activeSelf) return;

            // if (Input.GetMouseButtonDown(0) && m_CurrentItem && !m_InventoryGrid)
            // {
            //     DestroyItem();
            // }

            ItemIconDrag();

            if (!m_InventoryGrid)
            {
                itemHighlighter.Show(false);
                return;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateItem();
            }

            if (Input.GetMouseButtonDown(0))
            {
                LeftMouseButtonClick();
            }

            HandleHighlight();
        }

        // TODO: Window prompt if want to destroy item
        private void DestroyItem()
        {
            Destroy(m_CurrentItem.gameObject);
            m_CurrentItem = null;
        }

        private void RotateItem()
        {
            if (!m_CurrentItem) return;
            m_CurrentItem.Rotate();
        }

        private Item CreateItem(ItemData data)
        {
            var item = Instantiate(itemPrefab).GetComponent<Item>();
            item.SetData(data, 5);
            item.gameObject.SetActive(false);

            // TEMP to ensure that inventoryGrid is not null when picking up items
            // TODO: picking up items doesn't place correctly in the grid and size is wrong
            m_InventoryGrid = InventoryGrid;            
            
            var hasSpaceForItem = m_InventoryGrid.FindSpaceForItem(item); // null

            if (hasSpaceForItem is null)
            {
                Destroy(item.gameObject);
                Debug.Log("Inventory is full! (destroy item ? )".Color("red"));
                return null;
            }

            item.gameObject.SetActive(true);
            m_CurrentItem = item;
            m_ItemTransform = item.GetComponent<RectTransform>();
            m_ItemTransform.SetParent(canvasTransform);
            m_ItemTransform.SetAsLastSibling();
            return item;
        }

        public void InsertItem(ItemData data)
        {
            var item = CreateItem(data);

            if (!item) return;

            m_CurrentItem = null;
            m_ItemTransform = null;

            var positionOnGrid = m_InventoryGrid.FindSpaceForItem(item); // Also done in CreateItem()

            if (positionOnGrid is null) return;

            m_InventoryGrid.PlaceItem(item, positionOnGrid.Value.x, positionOnGrid.Value.y);
        }

        private void LeftMouseButtonClick()
        {
            var tilePos = GetTileGridPosition();

            if (m_CurrentItem is null)
            {
                PickupItem(tilePos);
            }
            else
            {
                PlaceItem(tilePos);
            }
        }

        private Vector2Int GetTileGridPosition()
        {
            var mousePos = Input.mousePosition;
            const int halfTileSize = InventoryGrid.k_TileSize / 2;

            if (m_CurrentItem)
            {
                mousePos.x -= (m_CurrentItem.Width - 1) * halfTileSize;
                mousePos.y += (m_CurrentItem.Height - 1) * halfTileSize;
            }

            return m_InventoryGrid.itemGrid.GetTilePosition(mousePos);
        }

        private void PlaceItem(Vector2Int tilePos)
        {
            if (!m_InventoryGrid.PlaceItem(m_CurrentItem, tilePos, ref m_OverlapItem))
            {
                return;
            }

            m_CurrentItem = null;
            m_ItemTransform = null;

            if (m_OverlapItem)
            {
                m_CurrentItem = m_OverlapItem;
                m_OverlapItem = null;
                m_ItemTransform = m_CurrentItem.GetComponent<RectTransform>();
                m_ItemTransform.SetAsLastSibling();
            }
        }

        private void PickupItem(Vector2Int tilePos)
        {
            // TODO: error when clicking on empty inventory space
            m_CurrentItem = m_InventoryGrid.itemGrid.PickupT(tilePos, m_InventoryGrid.ClearGridReference);

            if (m_CurrentItem)
            {
                m_ItemTransform = m_CurrentItem.GetComponent<RectTransform>();
                m_ItemTransform.SetAsLastSibling();
            }
        }

        private void ItemIconDrag()
        {
            if (m_ItemTransform)
            {
                m_ItemTransform.position = Input.mousePosition;
            }
        }

        // Highlight tiles where item will be placed
        private void HandleHighlight()
        {
            var tilePositionOnGrid = GetTileGridPosition();

            if (m_OldPosition == tilePositionOnGrid) return;
            m_OldPosition = tilePositionOnGrid;

            if (!m_CurrentItem)
            {
                m_ItemToHighlight = m_InventoryGrid.itemGrid.GetPositionOfT(tilePositionOnGrid);

                if (m_ItemToHighlight)
                {
                    itemHighlighter.SetSize(m_ItemToHighlight);
                    itemHighlighter.SetPosition(m_InventoryGrid, m_ItemToHighlight);
                    itemHighlighter.Show(true);
                }
                else
                {
                    itemHighlighter.Show(false);
                }
            }
            else
            {
                itemHighlighter.SetSize(m_CurrentItem);
                itemHighlighter.SetPosition(m_InventoryGrid, m_CurrentItem, tilePositionOnGrid);
                itemHighlighter.Show(m_InventoryGrid.BoundaryCheck(tilePositionOnGrid, m_CurrentItem));
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            InventoryGrid = GetComponent<InventoryGrid>();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryGrid = null;
        }
    }
}