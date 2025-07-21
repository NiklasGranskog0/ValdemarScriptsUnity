using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;

namespace Assets.Scripts.Units.Player
{
    public class PlayerAnimations : MonoBehaviour
    {
        private Vector3 m_MousePosition;
        private Vector3 m_InputMousePosition;
        private bool m_MouseTopLeft, m_MouseTopRight, m_MouseBottomLeft, m_MouseBottomRight;
        private bool m_MouseLeft, m_MouseRight, m_MouseTop, m_MouseBottom;
        private bool m_UpDir, m_LeftDir, m_RightDir, m_DownDir, m_UpLeftDir, m_UpRightDir, m_DownLeftDir, m_DownRightDir;
        private int m_ScreenWidth, m_ScreenHeight;
        public int upperBody, lowerBody = 1;

        private MouseScreenPosition m_MouseScreenPosition;
        //[SerializeField] private AnimationManager<PlayerAnim.Animations> animationManager;
        //[SerializeField] private AnimationManager<PlayerTjelvarAnimations.Animations> animationManager;
        [SerializeField] private AnimationManager<TjelvarAllAnimationsv2Animations.Animations> animationManager;

        private PlayerBase m_PlayerBase;
        private float m_PlayerVelocity;
        private float m_PlayerSprintSpeed;
        private float m_PlayerMovementSpeed;

        public void Initialize(PlayerBase playerBase, PlayerStats settings)
        {
            m_PlayerBase = playerBase;
            m_PlayerMovementSpeed = settings.movementSpeed;
            m_PlayerSprintSpeed = settings.sprintSpeed;

            //animationManager.Initialize(PlayerAnim.Animations.IdleWithWeapon001); // TEMP
            animationManager.Initialize(TjelvarAllAnimationsv2Animations.Animations.Idle1h); // TEMP
            m_MouseScreenPosition = new MouseScreenPosition(GameManager.Singleton.playerCamera);
        }

        public void OnUpdate(Vector3 playerDirection, Vector3 mousePosition)
        {
            m_PlayerVelocity = m_PlayerBase.PlayerVelocity;
            UpdatePlayerDirection(playerDirection);
            m_MouseScreenPosition.UpdateMousePosition(mousePosition);
            UpdateMouseQuadrants();

            CheckTopAnimations();
            CheckBottomAnimations();
        }

        private void CheckTopAnimations() => CheckMovementAnimations(upperBody);
        private void CheckBottomAnimations() => CheckMovementAnimations(lowerBody);

        private void UpdatePlayerDirection(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            m_UpDir = direction.z > 0f && direction.z != 0f && direction.x == 0f;
            m_DownDir = direction.z < 0f && direction.z != 0f && direction.x == 0f;
            m_LeftDir = direction.x < 0f && direction.x != 0f && direction.z == 0f;
            m_RightDir = direction.x > 0f && direction.x != 0f && direction.z == 0f;

            m_UpRightDir = direction.x > 0f && direction.z > 0f;
            m_UpLeftDir = direction.x < 0f && direction.z > 0f;
            m_DownRightDir = direction.x > 0f && direction.z < 0f;
            m_DownLeftDir = direction.x < 0f && direction.z < 0f;
        }

        private void UpdateMouseQuadrants()
        {
            var mousePositionInQuadrant = m_MouseScreenPosition.GetMousePositionQuadrant();
            m_MouseBottomRight = mousePositionInQuadrant.bottomRight;
            m_MouseBottomLeft = mousePositionInQuadrant.bottomLeft;
            m_MouseTopRight = mousePositionInQuadrant.topRight;
            m_MouseTopLeft = mousePositionInQuadrant.topLeft;

            m_MouseBottom = m_MouseBottomLeft || m_MouseBottomRight;
            m_MouseTop = m_MouseTopLeft || m_MouseTopRight;
            m_MouseLeft = m_MouseTopLeft || m_MouseBottomLeft;
            m_MouseRight = m_MouseTopRight || m_MouseBottomRight;
        }

        private void CheckMovementAnimations(int layer)
        {
            var walking = m_PlayerVelocity < m_PlayerSprintSpeed && m_PlayerVelocity > 0f;
            var running = m_PlayerVelocity > m_PlayerMovementSpeed;

            // TODO: Moving Straight doesn't need to check UpX, DownX? 
            // Looking towards X(left, right etc..) while moving in the opposite direction 
            var lookingLeft = m_MouseLeft && (m_RightDir || m_UpRightDir || m_DownRightDir);
            var lookingRight = m_MouseRight && (m_LeftDir || m_UpLeftDir || m_DownLeftDir);
            var lookingUp = m_MouseTop && (m_DownDir || m_DownLeftDir || m_DownRightDir);
            var lookingDown = m_MouseBottom && (m_UpDir || m_UpLeftDir || m_UpRightDir);

            var lookingLeftUp = m_MouseTopLeft && m_DownRightDir;
            var lookingLeftDown = m_MouseBottomLeft && m_UpRightDir;
            var lookingRightUp = m_MouseTopRight && m_DownLeftDir;
            var lookingRightDown = m_MouseBottomRight && m_UpLeftDir;

            var straight = (lookingLeft || lookingRight || lookingUp || lookingDown);
            var sideways = (lookingLeftUp || lookingLeftDown || lookingRightDown || lookingRightUp);

            if ((straight || sideways) && walking)
            {
                //animationManager.Play(PlayerAnim.Animations.WalkBackwards, layer);
                animationManager.Play(TjelvarAllAnimationsv2Animations.Animations.WalkBackwards1h, layer);
                return;
            }

            // ------------------------------------------------------------------------------------

            if ((straight || sideways) && running)
            {
                // animationManager.Play(PlayerAnim.Animations.RunBackwards, layer);
                animationManager.Play(TjelvarAllAnimationsv2Animations.Animations.RunBackwards, layer);
                return;
            }

            RunWalkIdle(layer);
        }

        private void RunWalkIdle(int layer)
        {
            if (m_PlayerVelocity > m_PlayerMovementSpeed)
            {
                // animationManager.Play(PlayerAnim.Animations.Run, layer);
                animationManager.Play(TjelvarAllAnimationsv2Animations.Animations.Runwith1h, layer);
            }

            if (m_PlayerVelocity < m_PlayerSprintSpeed && m_PlayerVelocity > 0f)
            {
                // animationManager.Play(PlayerAnim.Animations.Walk, layer);
                animationManager.Play(TjelvarAllAnimationsv2Animations.Animations.Walk, layer);
            }

            if (m_PlayerVelocity == 0f)
            {
                // animationManager.Play(PlayerAnim.Animations.IdleWithWeapon001, layer);
                animationManager.Play(TjelvarAllAnimationsv2Animations.Animations.Idle1h, layer);
            }
        }

        public void Play(TjelvarAllAnimationsv2Animations.Animations anim, int layer, bool lockLayer = false, bool overrideLock = false, float crossFade = 0.2f)
        {
            animationManager.Play(anim, layer, lockLayer, overrideLock, crossFade);
        }

        public void DelayedPlay(MonoBehaviour coroutineStarter, TjelvarAllAnimationsv2Animations.Animations anim, float delay, int layer,
            bool lockLayer = false, bool overrideLock = false, float crossFade = 0.2f)
        {
            animationManager.DelayedPlay(coroutineStarter, anim, delay, layer, lockLayer, overrideLock, crossFade);
        }
    }
}
