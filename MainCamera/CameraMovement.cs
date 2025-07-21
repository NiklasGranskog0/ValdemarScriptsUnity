using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;

namespace Assets.Scripts.MainCamera
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private CameraOptions cameraOptions;
        [SerializeField] private Transform targetTransform;

        private Transform m_Target;
        private Transform m_CameraTransform;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_Offset;
        private float m_SmoothTime;

        private void Awake()
        {
            m_SmoothTime = cameraOptions.smoothTime;
            m_Offset = transform.position - targetTransform.position;
        }

        private void LateUpdate()
        {
            if (GameManager.Singleton.CurrentGameState is not State.Gameplay) return;

            transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position + m_Offset, ref m_CurrentVelocity, m_SmoothTime);
        }
    }
}