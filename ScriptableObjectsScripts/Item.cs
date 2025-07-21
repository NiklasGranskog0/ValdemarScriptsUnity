using Assets.Scripts.Framework.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    // TODO: This is an INVENTORY ITEM
    public class Item : MonoBehaviour // Item Prefab
    {
        public ItemData GetItemData { get; private set; }
        [SerializeField] private TMP_Text stackCountText;
        public int stackCount;
        public int tilePositionX, tilePositionY; // items 0,0 pos (top left)
        public bool rotated;
        public SerializableGuid itemId;

        public int Width => rotated ? GetItemData.itemSize.y : GetItemData.itemSize.x;
        public int Height => rotated ? GetItemData.itemSize.x : GetItemData.itemSize.y;

        private int m_RotationNr;
        private RectTransform m_Transform;

        public void UpdateStackCount(int value)
        {
            if (int.TryParse(stackCountText.text, out var currentStack)) // ?
            {
                if (currentStack < 0)
                {
                    Debug.LogError("Stack value went below 0!");
                }
            }

            stackCount = value;
            stackCountText.text = stackCount.ToString();
            stackCountText.gameObject.SetActive(value > 1);
        }

        public void SetData(ItemData data, int initialStackCount = 1)
        {
            GetItemData = data;
            m_Transform = GetComponent<RectTransform>();
            
            GetComponent<Image>().sprite = data.itemSprite;

            var size = new Vector2
            {
                x = Width * 32,
                y = Height * 32,
            };
            m_Transform.sizeDelta = size;

            UpdateStackCount(initialStackCount);
            gameObject.name = data.itemName;
        }

        public void Rotate()
        {
            rotated = true;
            m_RotationNr++;

            if (m_RotationNr >= 4)
            {
                m_RotationNr = 0;
                rotated = false;
            }
            
            m_Transform.rotation = Quaternion.Euler(0f, 0f, m_RotationNr * -90f);
        }

        public void DestroyItem()
        {
            Destroy(gameObject);
        }
    }
}
