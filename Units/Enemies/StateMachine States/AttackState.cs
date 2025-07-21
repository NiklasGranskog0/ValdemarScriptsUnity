using System.Collections;
using Assets.Scripts.Framework.StateMachine;
using Assets.Scripts.Units.Player;
using UnityEngine;

namespace Assets.Scripts.Units.Enemies.StateMachine_States
{
    public class AttackState : BaseState
    {
        private readonly EnemyBase m_EntityBase;
        private readonly PlayerBase m_Player; // TODO: decouple
        private readonly float m_AttackCooldown;
        private bool m_CanAttack = true;
        
        public AttackState(EnemyBase entityBase, float attackCooldown)
        {
            m_EntityBase = entityBase;
            m_AttackCooldown = attackCooldown;
            m_Player = entityBase.Target.GetComponent<PlayerBase>();
        }

        private IEnumerator AttackCooldown()
        {
            m_CanAttack = false;
            yield return new WaitForSeconds(m_AttackCooldown);
            m_CanAttack = true;
        }

        public override void Update()
        {
            if (!m_CanAttack) return;
            
            m_Player.TakeDamage(m_EntityBase.damageSystem);
            m_EntityBase.StartCoroutine(AttackCooldown());
        }
    }
}
