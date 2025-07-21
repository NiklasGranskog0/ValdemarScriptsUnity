using UnityEngine;
using Assets.Scripts.Framework;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Items;
using Assets.Scripts.UI;
using Assets.Scripts.Units.Enemies;
using Assets.Scripts.Units.Player;
using System;
using System.Collections.Generic;
using Assets.Scripts.Framework.EventBus;
using Assets.Scripts.Framework.Structs;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Inventory;
using UnityEditor;
using ItemData = Assets.Scripts.ScriptableObjectsScripts.ItemData;
using PlayerSettings = Assets.Scripts.ScriptableObjectsScripts.PlayerSettings;

namespace Assets.Scripts.Managers
{
    public class UIManager : SingletonInstance<UIManager>
    {
        [SerializeField] private HealthBar healthBar;
        [SerializeField] private StaminaBar staminaBar;
        [SerializeField] private ExperienceBar experienceBar;
        [SerializeField] public InventoryController inventoryController;
        private PlayerSettings m_PlayerSettings;
        private PlayerBase m_Player;
        private GameObject m_CurrentLootWindow;
        private GameObject inventoryObject;
        private EventBinding<ExperienceEvent> m_ExperienceEvent;

        public void SetPlayerSettings(PlayerBase player, PlayerSettings settings)
        {
            m_ExperienceEvent = new EventBinding<ExperienceEvent>(HandleExperience);
            EventBus<ExperienceEvent>.Register(m_ExperienceEvent);

            m_PlayerSettings = settings;
            m_Player = player;

            healthBar.SetMaxHealth(settings.playerStats.healthPoints);

            staminaBar.StopSprintEvent += SprintStop;
            staminaBar.SetBaseSettings(settings.playerStats.staminaStruct);
        }

        public void SprintStart()
        {
            if (m_Player.PlayerVelocity <= 0f) return;

            if (staminaBar.GetStamina > 1f)
                m_Player.PlayerCurrentMoveSpeed = m_PlayerSettings.playerStats.sprintSpeed;

            staminaBar.SprintStart();
        }

        public void SprintStop()
        {
            m_Player.PlayerCurrentMoveSpeed = m_PlayerSettings.playerStats.movementSpeed;
            staminaBar.SprintStop();
        }

        public void TakeDamage(float damage)
        {
            var currentHealth = healthBar.slider.value;
            currentHealth -= damage;

            healthBar.SetCurrentHealth(currentHealth);

            if (!healthBar.isRegenerating)
                StartCoroutine(healthBar.RegenHealth());

            if (currentHealth > 0f) return;
            Debug.Log("Player Died!");
            #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
            #endif
            GameManager.Singleton.UpdateGameState(State.Lose);
        }

        public void OpenInventory()
        {
            if (inventoryObject is null)
            {
                inventoryObject = inventoryController.gameObject;
            }

            inventoryObject.SetActive(!inventoryObject.activeInHierarchy);
        }

        Dictionary<Guid, GameObject> LootWindowsInScene = new();

        public void OpenLootWindow(EnemyBase enemyBase)
        {
            if (!enemyBase.CanBeLooted) return;

            var enemyWindow = enemyBase.GetLootWindowObject;
            var enemyLootData = GetLootWindowItemDataAsSpan(enemyWindow);
            var guid = enemyWindow.GetComponent<LootWindow>().windowGUID.ToGuid();

            Span<ItemData> currentLootData;

            // LootWindowInScene[guid] = enemy window
            if (LootWindowsInScene.TryGetValue(guid, out var value) && ALootWindowIsOpen)
            {
                // If we open a different loot window while another one is open
                if (m_CurrentLootWindow != value)
                {
                    currentLootData = GetLootWindowItemDataAsSpan(m_CurrentLootWindow);
                    // currentLootData = GetLootWindowItemData(m_CurrentLootWindow);
                    CloseLootWindow(m_CurrentLootWindow, currentLootData);

                    m_CurrentLootWindow = LootWindowsInScene[guid];
                    SetLootWindowDataAsSpan(m_CurrentLootWindow, enemyLootData);
                    m_CurrentLootWindow.SetActive(true);
                    return;
                }

                // If we have a open loot window and we try opening it again = close it
                if (m_CurrentLootWindow == value)
                {
                    currentLootData = GetLootWindowItemDataAsSpan(m_CurrentLootWindow);
                    // currentLootData = GetLootWindowItemData(m_CurrentLootWindow);
                    CloseLootWindow(m_CurrentLootWindow, currentLootData);
                    return;
                }
            }

            // If we open a loot window that was already opened before
            if (LootWindowsInScene.TryGetValue(guid, out var value1) && !ALootWindowIsOpen)
            {
                m_CurrentLootWindow = value1;
                SetLootWindowDataAsSpan(m_CurrentLootWindow, GetLootWindowItemDataAsSpan(m_CurrentLootWindow));
                m_CurrentLootWindow.SetActive(true);
                return;
            }

            // If we open a different loot window while another one is open, but the new one is not instantiated yet
            if (m_CurrentLootWindow != null)
            {
                if (!LootWindowsInScene.TryGetValue(guid, out var _) && ALootWindowIsOpen)
                {
                    currentLootData = GetLootWindowItemDataAsSpan(m_CurrentLootWindow);
                    // currentLootData = GetLootWindowItemData(m_CurrentLootWindow);
                    CloseLootWindow(m_CurrentLootWindow, currentLootData);
                }
            }

            m_CurrentLootWindow = Instantiate(enemyWindow, transform); // Create Loot Window in UI
            SetLootWindowDataAsSpan(m_CurrentLootWindow, enemyLootData); // Set ItemData of dropped items in new loot window

            var currentLootWindowComp = m_CurrentLootWindow.GetComponent<LootWindow>(); // Get the Loot Window Component
            currentLootWindowComp.CreateItemsInLootWindow(); // Create items for the UI Loot Window
            currentLootWindowComp
                .SetEnemyBase(enemyBase); // Set enemy base of loot window, to send when loot window has been destroyed

            LootWindowsInScene.Add(guid, m_CurrentLootWindow); // Add the loot window to Dictionary with unique ID
        }

        private void CloseLootWindow(GameObject windowObj, Span<ItemData> itemDataSpan)
        {
            if (itemDataSpan.Length < 1)
            {
                windowObj.SetActive(false);
                GetEnemyBaseFromLootWindow(windowObj).Reset();
                RemoveWindowObjectInDictionary(windowObj);
            }
            else
            {
                windowObj.SetActive(false);
            }
        }

        private bool ALootWindowIsOpen => m_CurrentLootWindow.activeInHierarchy;

        private Span<ItemData> GetLootWindowItemDataAsSpan(GameObject obj)
        {
            return obj.TryGetComponent<LootWindow>(out var window)
                ? window.ItemDataOfDroppedItems.ToArray().AsSpan()
                : null;
        }

        private EnemyBase GetEnemyBaseFromLootWindow(GameObject windowObj) =>
            windowObj.GetComponent<LootWindow>().Enemy;

        
        private bool SetLootWindowDataAsSpan(GameObject obj, Span<ItemData> itemDataSpan)
        {
            if (obj.TryGetComponent<LootWindow>(out var window))
            {
                window.SetItemDataToLootWindowAsSpan(itemDataSpan);
            }

            return false;
        }

        private bool RemoveWindowObjectInDictionary(GameObject gObj)
        {
            foreach (var key in LootWindowsInScene)
            {
                if (LootWindowsInScene[key.Key] == gObj)
                {
                    LootWindowsInScene.Remove(key.Key);
                    return true;
                }
            }

            return false;
        }

        // EventBus way
        private void HandleExperience(ExperienceEvent experienceEvent)
        {
            experienceBar.AddExp(experienceEvent.experience);
        }

        // Singleton way
        // public void AddExperience(float amount)
        // {
        //     experienceBar.AddExp(amount);
        // } 
    }
}