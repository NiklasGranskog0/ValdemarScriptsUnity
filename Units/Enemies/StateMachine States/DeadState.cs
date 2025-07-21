using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.Framework.StateMachine;
using Assets.Scripts.Items;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Units.Enemies.StateMachine_States
{
    public class DeadState : BaseState
    {
        private GameObjectPooler m_GameObjectPooler;
        private EnemyHandler m_EnemyHandler;
        private readonly RagDollThings m_RagDoll;
        private readonly EnemyBase m_EntityBase;
        private Vector3 m_DropLocation;
        private List<ItemData> m_ItemDropList;

        public DeadState(RagDollThings ragDoll, EnemyBase entityBase)
        {
            m_RagDoll = ragDoll;
            m_EntityBase = entityBase;
        }

        public override void OnEnter()
        {
            m_GameObjectPooler = ServiceLocator.ForSceneOf(m_EntityBase).TryGet<GameObjectPooler>();
            m_EnemyHandler = ServiceLocator.Global.TryGet<EnemyHandler>();
            m_DropLocation = m_RagDoll.ragDollRoot.position;
            m_ItemDropList = new List<ItemData>();

            DestroyEntity();
        }

        /** TODO:
         * add force depending from direction of attack, setting for amount of totalForce = (Force * RB.Mass)
         */
        private void DestroyEntity()
        {
            m_EntityBase.IsDead = true;
            m_EnemyHandler.RemoveEnemy(m_EntityBase);
            m_EntityBase.StartCoroutine(CancelKnockBack());
        }

        private IEnumerator CancelKnockBack() // ?? 
        {
            // TODO: despawn when rag doll effect is over
            m_RagDoll.EnableRagDoll(5f);
            yield return new WaitForSeconds(5f);

            DropLoot();
        }

        private void DropLoot()
        {
            DropObject(ObjectEnum.ExpObject); // Drop Exp

            if (m_EntityBase.dropTable.ChanceToDropItem > Random.Range(0f, 1f))
            {
                AddItemToItemDropList();
                AddItemToItemDropList();
            }

            SetItemsInLootWindow();
        }

        private void AddItemToItemDropList()
        {
            ItemData itemData = null;

            if (GetItemData(out var data))
            {
                itemData = data;
            }

            if (itemData is null) return;
            m_ItemDropList.Add(itemData);
        }

        private bool GetItemData(out ItemData data)
        {
            var totalPercent = 0f;
            var totalChance = 0f;
            var weight = 1f;
            data = null;
            var rnd = Random.Range(0f, 1f);

            foreach (var itemData in m_EntityBase.dropTable.droppableItems)
            {
                totalPercent += itemData.DropChance;
            }

            // If total percent of getting an item is more than 100 % then we need to
            // reduce the total chance of every item, but still keep the 100 % of getting an item
            if (totalPercent > 1) weight = 1 / totalPercent;

            foreach (var itemData in m_EntityBase.dropTable.droppableItems)
            {
                totalChance += itemData.DropChance * weight;

                if (totalChance > rnd)
                {
                    data = itemData;
                    return true;
                }
            }

            // If total percent is less than 100 % it is possible we will not get an item
            return false;
        }

        private void SetItemsInLootWindow()
        {
            var drops = m_ItemDropList.ToArray().AsSpan();

            if (m_EntityBase.lootWindowObject.TryGetComponent<LootWindow>(out var window))
            {
                foreach (var item in drops)
                {
                    window.AddItemDataToLootWindow(item);
                }
            }

            m_ItemDropList.Clear();

            m_EntityBase.CanBeLooted = true;
        }

        private void DropObject(ObjectEnum obj)
        {
            var dropObject = m_GameObjectPooler.GetFromPool(obj, false);
            dropObject.transform.position = m_DropLocation;
            dropObject.SetActive(true);
        }
    }
}