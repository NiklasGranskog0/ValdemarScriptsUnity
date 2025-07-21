using System;
using System.Collections;
using Assets.Scripts.Battle;
using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using Assets.Scripts.Units.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Units.Player
{
    public class PlayerBase : MonoBehaviour
    {
        [SerializeField] protected PlayerSettings playerSettings;
        [SerializeField] private PlayerAnimations playerAnimations;
        [SerializeField] private DamageSystem damageSystem;
        [SerializeField] private Transform playerTransform;

        private Camera m_PlayerCamera;

        private bool m_CanAttack = true;
        private bool m_Sprint;
        private bool m_OpenInventory;

        public float PlayerVelocity { get; private set; }
        public float PlayerCurrentMoveSpeed { get; set; }
        private float m_MovementSpeed;

        private Vector3 m_MousePosition;
        private Vector3 m_InputMousePosition;
        private Vector3 m_PlayerDirection;

        private Ray m_MouseRay;

        public event Action AttackEvent;

        protected virtual void OnStart() { }

        private void Awake()
        {
            UIManager.Singleton.SetPlayerSettings(this, playerSettings);
            ServiceLocator.ForSceneOf(this).Register(playerAnimations, ServiceLevel.Scene, gameObject.scene.name);
        }

        private void Start()
        {
            damageSystem.damageInfo.damage = playerSettings.playerStats.initialDamage;
            m_MovementSpeed = playerSettings.playerStats.movementSpeed;
            PlayerCurrentMoveSpeed = m_MovementSpeed;

            playerSettings.SetGameplayInputs(MouseMovement, PlayerMovement, StopPlayerMovement,
                Attack, OpenInventory, SprintStart, SprintStop);

            playerSettings.SetUIInputs(LootDeadEnemy);

            playerAnimations.Initialize(this, playerSettings.playerStats);

            m_PlayerCamera = GameManager.Singleton.playerCamera;

            OnStart();
        }

        private void SprintStart() => UIManager.Singleton.SprintStart();
        private void SprintStop() => UIManager.Singleton.SprintStop();

        private void Update()
        {
            MousePositionRay();
            playerAnimations.OnUpdate(m_PlayerDirection, m_MousePosition);
            Move();
        }

        /* TODO: 
         * By moving the character by transform it will ignore collision validation?
         * Continuously walking into an object with collision will make the player character "flicker" 
         */
        private void Move()
        {
            PlayerVelocity = Mathf.Floor((m_PlayerDirection * PlayerCurrentMoveSpeed).magnitude);

            var forward = Quaternion.AngleAxis(45f, Vector3.up) * m_PlayerDirection;
            playerTransform.position += forward.normalized * (PlayerVelocity * Time.deltaTime);
        }

        private void MouseMovement(Vector2 mousePosition)
        {
            m_InputMousePosition = mousePosition;
            playerTransform.LookAt(new Vector3(m_MousePosition.x, transform.position.y, m_MousePosition.z));
        }

        private void MousePositionRay()
        {
            m_MouseRay = m_PlayerCamera.ScreenPointToRay(m_InputMousePosition);

            if (Physics.Raycast(m_MouseRay, out var hit, float.MaxValue, playerSettings.mouseLayer))
            {
                m_MousePosition = hit.point;
            }
        }

        private void StopPlayerMovement() => m_PlayerDirection = Vector3.zero;

        private void PlayerMovement(Vector2 direction)
        {
            m_PlayerDirection.x = direction.x;
            m_PlayerDirection.z = direction.y;
        }

        private void Attack()
        {
            if (!m_CanAttack) return;

            AttackEvent?.Invoke();
            StartCoroutine(AttackCooldown());
        }

        private void LootDeadEnemy()
        {
            // TODO: Dead enemy collider is not on the rag-doll, be able to click anywhere on rag-doll to loot
            if (Physics.Raycast(m_MouseRay, out var hit, float.MaxValue, playerSettings.enemyLayer))
            {
                if (hit.collider.TryGetComponent<EnemyBase>(out var enemy))
                {
                    if (enemy.IsDead) UIManager.Singleton.OpenLootWindow(enemy);
                }
            }
        }

        private void OpenInventory()
        {
            UIManager.Singleton.OpenInventory();
        }

        protected void DoAttack(IDamage enemy)
        {
            if (damageSystem.hitChance < Random.Range(0f, 1f))
                return;

            enemy.TakeDamage(damageSystem);
        }

        private IEnumerator AttackCooldown()
        {
            // TODO: Attack animation play length is longer than the attack cooldown

            m_CanAttack = false;
            yield return damageSystem.AttackCooldown;
            m_CanAttack = true;
        }

        public void TakeDamage(DamageSystem enemyDamageSystem) =>
            UIManager.Singleton.TakeDamage(damageSystem.TakeDamage(enemyDamageSystem));
    }
}