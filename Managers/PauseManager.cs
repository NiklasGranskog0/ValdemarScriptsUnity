using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class PauseManager : MonoBehaviour
    {
        [SerializeField] private GameObject pauseCanvas;
        [SerializeField] private InputReader inputReader;
        private bool m_Pause;

        private void Start()
        {
            inputReader.PauseEvent += PauseGame;
            inputReader.ResumeEvent += PauseGame;

            inputReader.EnableMenuInputs(false);
        }

        private void PauseGame()
        {
            m_Pause = !m_Pause;

            inputReader.EnableGameplayInputs(!m_Pause);
            inputReader.EnableMenuInputs(m_Pause);
            pauseCanvas.SetActive(m_Pause);

            Time.timeScale = m_Pause ? 0f : 1f;
            var state = m_Pause ? State.Pause : State.Gameplay;
            GameManager.Singleton.UpdateGameState(state);
        }
    }
}
