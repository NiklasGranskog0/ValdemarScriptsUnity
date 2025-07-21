using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Units.Player
{
    public class MouseScreenPosition
    {
        private readonly Camera m_PlayerCamera;
        private float m_HalfScreenWidth;
        private float m_HalfScreenHeight;

        public MouseScreenPosition(Camera playerCamera)
        {
            GameManager.Singleton.OnScreenSizeChangeEvent += UpdateScreenSize;

            m_PlayerCamera = playerCamera;
            UpdateScreenSize();
        }

        // TODO: Update on screen size change
        private void SetScreenSizeHalf(float screenWidth, float screenHeight)
        {
            m_HalfScreenWidth = screenWidth * 0.5f;
            m_HalfScreenHeight = screenHeight * 0.5f;
        }

        private void UpdateScreenSize()
        {
            SetScreenSizeHalf(Screen.width, Screen.height);
        }

        private Vector3 GetMousePosition { get; set; }

        public void UpdateMousePosition(Vector3 mouseGroundHit)
            => GetMousePosition = m_PlayerCamera.WorldToScreenPoint(mouseGroundHit);

        public (bool topRight, bool topLeft, bool bottomLeft, bool bottomRight) GetMousePositionQuadrant()
        {
            var top = GetMousePosition.y >= m_HalfScreenHeight;
            var right = GetMousePosition.x >= m_HalfScreenWidth;
            var left = GetMousePosition.x <= m_HalfScreenWidth;
            var bottom = GetMousePosition.y <= m_HalfScreenHeight;

            var topRight = top && right;
            var topLeft = top && left;
            var bottomLeft = bottom && left;
            var bottomRight = bottom && right;

            return (topRight, topLeft, bottomLeft, bottomRight);
        }
    }
}
