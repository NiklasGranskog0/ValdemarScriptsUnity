using System;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        public PlayerStats playerStats;
        public InputReader inputReader;
        public LayerMask groundLayer;
        public LayerMask enemyLayer;
        public LayerMask mouseLayer;
        public LayerMask uiLayer;

        public void SetGameplayInputs(
            Action<Vector2> mouseEvent,
            Action<Vector2> playerMovementEvent,
            Action stopPlayerMovementEvent,
            Action attackEvent,
            Action openInventoryEvent,
            Action sprintStart,
            Action sprintStop)
        {
            inputReader.MouseMovementEvent += mouseEvent;
            inputReader.MovementEvent += playerMovementEvent;
            inputReader.MovementStopEvent += stopPlayerMovementEvent;
            inputReader.AttackEvent += attackEvent;
            inputReader.InventoryOpenEvent += openInventoryEvent;
            inputReader.SprintStartEvent += sprintStart;
            inputReader.SprintStopEvent += sprintStop;
        }

        public void SetUIInputs(Action rightMouseClick, Action leftMouseClick = null)
        {
            inputReader.LeftClickEvent += leftMouseClick;
            inputReader.RightClickEvent += rightMouseClick;
        }
    }
}