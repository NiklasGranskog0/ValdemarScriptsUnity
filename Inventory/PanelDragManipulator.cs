using UnityEngine;
using UnityEngine.UIElements;
using MouseButton = UnityEngine.UIElements.MouseButton;

namespace Assets.Scripts.Inventory
{
    public class PanelDragManipulator : PointerManipulator
    {
        private bool m_IsDragging;
        private Vector2 m_Offset;
        
        public PanelDragManipulator()
        {
            activators.Add(new ManipulatorActivationFilter() {button = MouseButton.LeftMouse});
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }

        public void OnPointerDown(PointerDownEvent evt)
        {
            if (!CanStartManipulation(evt) || m_IsDragging) return;

            m_Offset = evt.localPosition;
            m_IsDragging = true;
            
            target.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        public void OnPointerMove(PointerMoveEvent evt)
        {
            if (!m_IsDragging || !target.HasPointerCapture(evt.pointerId)) return;

            var delta = evt.localPosition - (Vector3)m_Offset;
            target.transform.position += delta;
            evt.StopPropagation();
        }

        public void OnPointerUp(PointerUpEvent evt)
        {
            if (!CanStopManipulation(evt) || !m_IsDragging) return;

            m_IsDragging = false;
            target.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }
    }
}
