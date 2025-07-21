using System;
using System.Linq;
using Assets.Scripts.Framework.SceneManagement;
using Assets.Scripts.Framework.ServiceManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    [System.Serializable]
    public struct MenuButtons
    {
        public Button button;
        public string buttonName;

        public void SetButtonName() => button.name = buttonName;
    }

    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private MenuButtons[] menuButtons;

        private void Awake()
        {
            foreach (var button in menuButtons)
            {
                button.SetButtonName();
            }

            GetButton("TestScene").onClick.AddListener(() => LoadScene(SceneLoader.SceneGroupToLoad.Test));
            GetButton("VisbyFields").onClick.AddListener(() => LoadScene(SceneLoader.SceneGroupToLoad.VisbyFields));
        }

        private async void LoadScene(SceneLoader.SceneGroupToLoad sceneGroupToLoad)
        {
            ServiceLocator.Global.Get<SceneLoader>(out var sceneLoader);
            await sceneLoader.LoadSceneGroupByIndex((int)sceneGroupToLoad);
        }

        private Button GetButton(string buttonName)
        {
            return (from menuButton in menuButtons
                where menuButton.buttonName.Equals(buttonName)
                select menuButton.button).FirstOrDefault();
        }

        private void OnDisable()
        {
            foreach (var menuButton in menuButtons)
            {
                menuButton.button.onClick.RemoveAllListeners();
            }
        }
    }
}