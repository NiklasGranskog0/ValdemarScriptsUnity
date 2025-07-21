using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.MainCamera
{
    public class DeActivateSpawnerInView : MonoBehaviour
    {
        private Camera m_MainCamera;

        private void Start()
        {
            m_MainCamera = GameManager.Singleton.playerCamera;
        }

        // TODO: Don't run when player is not moving, might also be a temporary depending on how enemies should spawn
        private void Update()
        {
            if (!m_MainCamera) return;

            foreach (Transform child in transform)
            {
                var point = m_MainCamera.WorldToScreenPoint(child.position);

                if (point.x > Screen.width || point.x < 0f || point.y > Screen.height || point.y < 0f)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}