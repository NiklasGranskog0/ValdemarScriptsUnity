using Assets.Scripts.Framework.ServiceManagement;
using UnityEngine;

namespace Assets.Scripts.Units.Player.AnimationStates
{
    public class Melee1HSwingAltState : StateMachineBehaviour
    {
        [SerializeField] private TjelvarAllAnimationsv2Animations.Animations resetAnimation;
        private PlayerBase m_PlayerBase;
        private PlayerAnimations m_PlayerAnim;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!m_PlayerBase) m_PlayerBase = animator.GetComponentInParent<PlayerBase>();
            if (!m_PlayerAnim) m_PlayerAnim = ServiceLocator.ForSceneOf(m_PlayerBase).TryGet<PlayerAnimations>();

            m_PlayerAnim.DelayedPlay(m_PlayerBase, resetAnimation, stateInfo.length, layerIndex, false, true);
        }
    }
}
