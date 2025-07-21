using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Extensions;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using Assets.Scripts.Units.Enemies;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class LootWindow : MonoBehaviour
    {
        private GameObject m_LootWindow;

        public GameObject itemContainer;
        public LootableItem lootTemplate;
        public List<ItemData> ItemDataOfDroppedItems { get; private set; }
        public SerializableGuid windowGUID;
        public EnemyBase Enemy { get; private set; }

        // Unity Call
        private void Awake()
        {
            m_LootWindow = gameObject;
            windowGUID = SerializableGuid.NewGuid;
            ItemDataOfDroppedItems = new();
        }

        private string SetStatsText(int str, int sta, int agi, int @int, int spr, int arm)
        {
            var statsText = string.Empty;

            if (str > 0) statsText += $"+ {str} Strength\n";
            if (sta > 0) statsText += $"+ {sta} Stamina\n";
            if (agi > 0) statsText += $"+ {agi} Agility\n";
            if (@int > 0) statsText += $"+ {@int} Intellect\n";
            if (spr > 0) statsText += $"+ {spr} Spirit\n";
            if (arm > 0) statsText += $"+ {arm} Armor\n";

            return statsText;
        }

        public void AddItemDataToLootWindow(ItemData itemData)
        {
            ItemDataOfDroppedItems.Add(itemData);
        }

        public void SetItemDataToLootWindow(List<ItemData> itemDatas) => ItemDataOfDroppedItems = itemDatas;
        public void SetItemDataToLootWindowAsSpan(Span<ItemData> itemData) => ItemDataOfDroppedItems = itemData.ToArray().ToList();

        public void SetEnemyBase(EnemyBase enemyBase) => Enemy = enemyBase;

        public void CreateItemsInLootWindow()
        {
            foreach (var itemData in ItemDataOfDroppedItems)
            {
                CreateNewItemInLootWindow(itemData, itemData.itemValues);
            }
        }

        private void CreateNewItemInLootWindow(ItemData itemData, DroppableItemValues itemValues)
        {
            var lootItem = CreateNewLootItem(itemData);

            lootItem.itemName.text = itemData.itemName.SetColorByRarity(itemData.itemRarity);
            lootItem.itemIcon.sprite = itemData.itemSprite;
            lootItem.itemDescription.itemName.text = itemData.itemName.SetColorByRarity(itemData.itemRarity);

            lootItem.itemDescription.itemSlot.text = itemValues.itemSlot.ToString();

            if (itemValues.weaponType != ItemEnums.WeaponType.None)
            {
                lootItem.itemDescription.itemSlot.text = itemValues.itemSlot.WeaponItemSlotToString();
                lootItem.itemDescription.itemType.text = itemValues.weaponType.ToString();
                lootItem.itemDescription.weaponDamage.text = itemValues.damage;
                lootItem.itemDescription.weaponDps.text = $"({itemValues.damagePerSecond}) damage per second";
                lootItem.itemDescription.weaponSpeed.text = $"Speed {itemValues.speed}";
            }

            var statsText = SetStatsText(itemValues.strength, itemValues.stamina, itemValues.agility, itemValues.intellect, itemValues.spirit,
                itemValues.armor);

            lootItem.itemDescription.itemStats.text = statsText;
        }

        private LootableItem CreateNewLootItem(ItemData itemData)
        {
            var item = Instantiate(lootTemplate, itemContainer.transform);
            item.itemData = itemData;
            item.ItemOnClick += ItemOnClick;

            item.itemDescription.ResetDescription();
            return item;
        }

        private void ItemOnClick(LootableItem item)
        {
            Debug.Log($"Picked Up: [{item.itemName.text}]");

            UIManager.Singleton.inventoryController.InsertItem(item.itemData);

            ItemDataOfDroppedItems.Remove(item.itemData);
            Destroy(item.gameObject);
        }


        // This is always false, it's called when clicking the X button on the loot window
        public void OpenLootWindow(bool open)
        {
            m_LootWindow.SetActive(open);

            if (open == false)
            {
                CloseLootWindow(gameObject, ItemDataOfDroppedItems);
            }
        }

        private void CloseLootWindow(GameObject windowObj, List<ItemData> itemDatas)
        {
            if (itemDatas.Count < 1)
            {
                windowObj.SetActive(false);
                Enemy.Reset();
            }
            else
            {
                windowObj.SetActive(false);
            }
        }
    }
}
