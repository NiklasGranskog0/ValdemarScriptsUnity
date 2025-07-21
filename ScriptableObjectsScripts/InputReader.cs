using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    // TODO: IInventoryActions
    [CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/InputReader")]
    public class InputReader : ScriptableObject,
        PlayerInputActions.IGameplayActions,
        PlayerInputActions.IMenuActions,
        PlayerInputActions.IUIActions
    {
        private PlayerInputActions m_PlayerInputActions;

        private void OnEnable()
        {
            m_PlayerInputActions ??= new();
            m_PlayerInputActions.Gameplay.SetCallbacks(this);
            m_PlayerInputActions.Gameplay.Enable();
            
            m_PlayerInputActions.Menu.SetCallbacks(this);
            m_PlayerInputActions.Menu.Enable();
            
            m_PlayerInputActions.UI.SetCallbacks(this);
            m_PlayerInputActions.UI.Enable();
        }

        private void OnDisable()
        {
            m_PlayerInputActions.Gameplay.Disable();
            m_PlayerInputActions.Menu.Disable();
            m_PlayerInputActions.UI.Disable();
        }

        public event Action AttackEvent = delegate {  };
        public event Action AttackCanceledEvent = delegate {  };
        public event Action PauseEvent = delegate {  };
        public event Action<Vector2> MouseMovementEvent = delegate {  };
        public event Action InventoryOpenEvent = delegate {  };
        public event Action SprintStartEvent = delegate {  };
        public event Action SprintStopEvent = delegate {  };
        public event Action<Vector2> MovementEvent = delegate {  };
        public event Action MovementStopEvent = delegate {  };
        public event Action ResumeEvent = delegate {  };
        public event Action LeftClickEvent = delegate {  };
        public event Action RightClickEvent = delegate {  };
        
        public void OnMovement(InputAction.CallbackContext context)
        {
            if (context.performed)
                MovementEvent.Invoke(context.ReadValue<Vector2>());
            
            if (context.canceled)
                MovementStopEvent.Invoke();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                SprintStartEvent.Invoke();
            
            if (context.phase == InputActionPhase.Canceled)
                SprintStopEvent.Invoke();
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                InventoryOpenEvent.Invoke();
        }

        public void OnMouseMovement(InputAction.CallbackContext context)
        {
            MouseMovementEvent.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                PauseEvent.Invoke();
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    break;
                case InputActionPhase.Started:
                    AttackEvent.Invoke();
                    break;
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Canceled:
                    AttackCanceledEvent.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public void OnResume(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                ResumeEvent.Invoke();
        }
        
        public void OnLeftMouseClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                LeftClickEvent.Invoke();
        }

        public void OnRightMouseClick(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                RightClickEvent.Invoke();
        }

        private void DisableGameplayInputs() => m_PlayerInputActions.Gameplay.Disable();
        private void EnableGameplayInputs() => m_PlayerInputActions.Gameplay.Enable();
        private void DisableMenuInputs() => m_PlayerInputActions.Menu.Disable();
        private void EnableMenuInputs() => m_PlayerInputActions.Menu.Enable();

        public void EnableGameplayInputs(bool enable) 
        {
            if (enable) 
                EnableGameplayInputs();
            else DisableGameplayInputs();
        }
        
        public void EnableMenuInputs(bool enable) 
        {
            if (enable) 
                EnableMenuInputs();
            else DisableMenuInputs();
        }
    }
}