using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class EndSceneManager : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Singleton.GameStateEvent += EndScene;
        }

        private void OnDisable() => GameManager.Singleton.GameStateEvent -= EndScene;

        private void EndScene(State state)
        {
            switch (state)
            {
                case State.Exit:
                    ExitScene();
                    break;
                case State.Lose:
                    LoseScene();
                    break;
                case State.Win:
                    WinScene();
                    break;
            }
        }

        private void WinScene()
        {
            Debug.Log("Win Scene");
        }

        private void LoseScene()
        {
            Debug.Log("Lose Scene");
            Time.timeScale = 0f;
        }

        private void ExitScene()
        {
            Debug.Log("Exit Scene");
        }
    }
}
