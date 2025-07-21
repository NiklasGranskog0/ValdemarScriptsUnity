/* using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Assets.Scripts.Managers
{
    [Obsolete("Use Scene Loader")]
    public class SceneManager : Singleton<SceneManager>
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;
        [SerializeField] private List<SceneAsset> sceneToLoad;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private AudioListener audioListener;
        [SerializeField] private InputSystemUIInputModule inputSystemUIInputModule;
        private List<SceneAsset> m_LoadScenes;
        private Scene m_CurrentScene;
        private float m_TotalLoadProgress;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(loadingScreen);
            DontDestroyOnLoad(eventSystem);
            DontDestroyOnLoad(audioListener);
            progressBar = loadingScreen.GetComponentInChildren<Slider>(true);
            m_LoadScenes = new();
        }

        public void LoadScene(List<SceneIndex> scenesToLoad, SceneIndex sceneToSetActive)
        {
            m_CurrentScene = UnitySceneManager.GetActiveScene();

            loadingScreen.SetActive(true);
            
            foreach (var index in scenesToLoad)
            {
                m_LoadScenes.Add(sceneToLoad[(int)index]);
            }

            StartCoroutine(SceneLoadProgress(sceneToSetActive));
        }

        private IEnumerator SceneLoadProgress(SceneIndex sceneToSetActive)
        {
            progressBar.maxValue = m_LoadScenes.Count + 1f;
            
            foreach (var scene in m_LoadScenes)
            {
                var operation = UnitySceneManager.LoadSceneAsync(scene.name, LoadSceneMode.Additive);

                while (!operation.isDone)
                {
                    progressBar.value += operation.progress / 0.9f;
                    yield return null;
                }
            }
            
            DestroyExtra();
            
            var unloadScene = UnitySceneManager.UnloadSceneAsync(m_CurrentScene);

            while (!unloadScene.isDone)
            {
                progressBar.value += unloadScene.progress / 0.9f;
                yield return null;
            }
            
            var sceneToSet = UnitySceneManager.GetSceneByBuildIndex((int)sceneToSetActive);
            UnitySceneManager.SetActiveScene(sceneToSet);
            
            loadingScreen.SetActive(false);
        }

        private void DestroyExtra()
        {
            if (!UnitySceneManager.GetSceneByName("Visby_fields_map").isLoaded) return;
            inputSystemUIInputModule.enabled = false;
            eventSystem.enabled = false;
            audioListener.enabled = false;
            
            Destroy(inputSystemUIInputModule);
            Destroy(eventSystem);
            Destroy(audioListener);
        }
    }
    
    public enum SceneIndex
    {
        MainMenuScene = 0,
        TestScene = 1,
        UIScene = 2,
        VisbyFields = 3,
    }
}
 */