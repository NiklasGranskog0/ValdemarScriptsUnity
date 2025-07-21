using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Units.Player
{
    // TODO: This is basically a WeaponClass
    public class PlayerTemp : PlayerBase // TEMP CLASS 
    {
        [SerializeField] private Transform axeWeaponOrigin;
        private PlayerAnimations m_PlayerAnim;
        
        protected override void OnStart()
        {
            m_PlayerAnim = ServiceLocator.ForSceneOf(this).TryGet<PlayerAnimations>();
            
            AttackEvent += AxeAttack;
        }

        private void AxeAttack()
        {
            // m_PlayerAnim.Play(PlayerAnim.Animations.Melee1HSwingAlt, m_PlayerAnim.upperBody, true);
            m_PlayerAnim.Play(TjelvarAllAnimationsv2Animations.Animations.OnehCombo, m_PlayerAnim.upperBody, true);
            
            // TODO: Non alloc box cast around weapon, Double check hit-box
            var hits = Physics.BoxCastAll(axeWeaponOrigin.position, Vector3.one * 2f,
                transform.forward, Quaternion.identity, 1f, playerSettings.enemyLayer);
            
            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent<IDamage>(out var enemy))
                {
                    DoAttack(enemy);
                }
            }
        }

        // Showing where box cast is
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(axeWeaponOrigin.position, Vector3.one);
        }
    }
}
