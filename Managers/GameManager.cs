using System;
using Assets.Scripts.Framework;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.UI;
using Assets.Scripts.Units.Enemies;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameManager : SingletonInstance<GameManager>
    {
        public GameObject playerCharacter;
        public Camera playerCamera;
        private EnemyHandler m_EnemyHandler;
        private int m_ScreenWidth;
        private int m_ScreenHeight;

        //TODO: GameManager should handle game states

        public State CurrentGameState { get; private set; }
        public event Action<State> GameStateEvent;
        public event Action OnScreenSizeChangeEvent = delegate { };

        private void Awake()
        {
            // Application.targetFrameRate = 60;
            CurrentGameState = State.Gameplay; // Temp
            m_EnemyHandler = new EnemyHandler(); // Sets up service locator for enemy handler
        }

        private void Start()
        {
            ServiceLocator.Global.Get<GameTimer>(out var timer);
            timer.StartTimer = true;

            m_ScreenHeight = Screen.height;
            m_ScreenWidth = Screen.width;
        }

        private void Update()
        {
            if (Screen.currentResolution.width != m_ScreenWidth || Screen.currentResolution.height != m_ScreenHeight)
            {
                OnScreenSizeChangeEvent.Invoke();

                // Screen.currentResolution = 1920 x 1080
                // Screen.width/height = The actual size of the game view
                m_ScreenWidth = Screen.width;
                m_ScreenHeight = Screen.height;
            }
        }

        public void UpdateGameState(State state)
        {
            if (CurrentGameState == state) return;

            CurrentGameState = state;

            GameStateEvent?.Invoke(state);
        }
    }
}
