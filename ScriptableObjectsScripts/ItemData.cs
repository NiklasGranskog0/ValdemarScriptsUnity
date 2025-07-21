using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.Extensions;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "New_Inventory_ItemData", menuName = "ScriptableObjects/Items/Create New Item Data")]
    public class ItemData : ScriptableObject
    {
        // Values that will be updated for items should be in Item.cs
        
        public string itemName;
        public string description;
        public int maxStack;
        public Vector2Int itemSize;
        public ItemRarity itemRarity;
        public Sprite itemSprite;

        [Header("Item GUID")]
        public SerializableGuid itemID = SerializableGuid.NewGuid;
        public void AssignNewGuid() => itemID = SerializableGuid.NewGuid;
        
        public Mesh itemMesh;
        public Material itemMaterial;
        
        public DroppableItemValues itemValues;
        
        [Header("Drop chance in percent (%)")]
        [SerializeField] private int dropChance;
        public float DropChance => dropChance * 0.01f;
    }
}