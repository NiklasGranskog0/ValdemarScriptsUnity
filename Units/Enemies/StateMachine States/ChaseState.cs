using Assets.Scripts.Framework.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Units.Enemies.StateMachine_States
{
    public class ChaseState : BaseState
    {
        private readonly NavMeshAgent m_Agent;
        private readonly EnemyBase m_EntityBase;

        public ChaseState(NavMeshAgent navMeshAgent, EnemyBase entityBase)
        {
            m_Agent = navMeshAgent;
            m_EntityBase = entityBase;
        }

        public override void Update()
        {
            if (m_EntityBase.IsDead) return;
            MoveTowardsTarget();
        }
        
        private void MoveTowardsTarget()
        {
            if (m_EntityBase.HealthPoints > 0f && m_EntityBase.Target)
            {
                m_Agent.SetDestination(m_EntityBase.Target.transform.position);
            }
        }
    }
}
