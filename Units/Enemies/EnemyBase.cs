using Assets.Scripts.Battle;
using Assets.Scripts.Framework.StateMachine;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using Assets.Scripts.Units.Enemies.StateMachine_States;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Units.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyBase : MonoBehaviour, IDamage // TODO: useless interfaces ? 
    {
        [SerializeField] private EnemyStats stats;
        [SerializeField] private NavMeshAgent owner;
        [SerializeField] private RagDollThings ragDollThings;
        public LayerMask playerLayer;
        public DamageSystem damageSystem;
        public DropTable dropTable;
        public GameObject lootWindowObject;
        public GameObject GetLootWindowObject => lootWindowObject;

        private bool InRange { get; set; }
        public bool IsDead { get; set; }
        public bool CanBeLooted { get; set; }
        public float HealthPoints { get; private set; }
        public GameObject Target { get; private set; }
        private Collider[] m_OverlapHits;

        private StateMachine m_StateMachine;
        private ChaseState m_ChaseState;
        private AttackState m_AttackState;
        private DeadState m_DeadState;

        public NavMeshPathStatus status;

        private void Awake()
        {
            CanBeLooted = false;
            

            // State Machine
            m_StateMachine = new StateMachine();

            HealthPoints = stats.healthPoints;
            owner.speed = stats.movementSpeed;
            damageSystem.damageInfo.damage = stats.initialDamage;
            Target = GameManager.Singleton.playerCharacter; // TODO: decouple player from enemy-base
            m_OverlapHits = new Collider[1];

            // Declare states
            m_ChaseState = new ChaseState(owner, this);
            m_DeadState = new DeadState(ragDollThings, this);
            m_AttackState = new AttackState(this, stats.attackCooldown);

            // Define transitions between states
            m_StateMachine.AddTransition(m_ChaseState, m_DeadState, new FuncPredicate(() => HealthPoints <= 0f));
            m_StateMachine.AddTransition(m_DeadState, m_ChaseState, new FuncPredicate(() => !IsDead && HealthPoints >= stats.healthPoints));
            m_StateMachine.AddTransition(m_ChaseState, m_AttackState, new FuncPredicate(() => InRange));
            m_StateMachine.AddTransition(m_AttackState, m_ChaseState, new FuncPredicate(() => !InRange));

            // Set start state
            m_StateMachine.SetState(m_ChaseState);
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            HealthPoints = stats.healthPoints;
            CanBeLooted = false;
            IsDead = false;
        }

        private void Update()
        {
            m_StateMachine.Update();
            status = owner.pathStatus;
            InPlayerRange();
        }

        private void InPlayerRange()
        {
            var h = Physics.OverlapBoxNonAlloc(ragDollThings.ragDollRoot.transform.position, transform.localScale / 2,
                m_OverlapHits, Quaternion.identity, playerLayer);

            InRange = h > 0;
        }

        public void TakeDamage(DamageSystem enemyDamageSystem)
        {
            if (IsDead) return;

            var damage = damageSystem.TakeDamage(enemyDamageSystem);
            HealthPoints -= damage;
        }
    }
}
