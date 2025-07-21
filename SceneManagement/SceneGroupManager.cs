using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Framework.SceneManagement
{
    public class SceneGroupManager
    {
        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnLoaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        private SceneGroup m_ActiveSceneGroup;

        private readonly AsyncOperationHandleGroup m_AsyncOperationHandleGroup = new(10);
        private readonly AsyncOperationGroup m_AsyncOperationGroup = new(10);

        public async Task LoadScenes(SceneGroup group, IProgress<float> loadingProgress, bool reloadDuplicateScenes = false)
        {
            m_ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            var sceneCount = SceneManager.sceneCount;

            for (int i = 0; i < sceneCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = m_ActiveSceneGroup.scenes.Count;

            for (int i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.scenes[i];

                if (!reloadDuplicateScenes && loadedScenes.Contains(sceneData.Name)) continue;

                if (sceneData.reference.State == SceneReferenceState.Regular)
                {
                    var asyncOperation = SceneManager.LoadSceneAsync(sceneData.reference.Path, LoadSceneMode.Additive);
                    m_AsyncOperationGroup.asyncOperations.Add(asyncOperation);
                }
                else if (sceneData.reference.State == SceneReferenceState.Addressable)
                {
                    var sceneHandle = Addressables.LoadSceneAsync(sceneData.reference.Path, LoadSceneMode.Additive);
                    m_AsyncOperationHandleGroup.handles.Add(sceneHandle);
                }

                OnSceneLoaded.Invoke(sceneData.Name);
            }

            while (!m_AsyncOperationGroup.IsDone || !m_AsyncOperationHandleGroup.IsDone)
            {
                loadingProgress.Report((m_AsyncOperationGroup.Progress + m_AsyncOperationHandleGroup.Progress) / 2);
                await Task.Delay(100);
            }

            var activeScene = SceneManager.GetSceneByName(m_ActiveSceneGroup.FindSceneByType(SceneType.ActiveScene));

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnSceneGroupLoaded.Invoke();
        }

        public async Task UnloadScenes(bool unloadUnusedAssets = false)
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;
            var sceneCount = SceneManager.sceneCount;

            for (int i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName.Equals(activeScene) || sceneName == "Bootstrapper") continue;
                if (m_AsyncOperationHandleGroup.handles.Any(h => h.IsValid() && h.Result.Scene.name == sceneName)) continue;
                scenes.Add(sceneName);
            }

            var asyncOperationGroup = new AsyncOperationGroup(scenes.Count);

            foreach (var scene in scenes)
            {
                var asyncOperation = SceneManager.UnloadSceneAsync(scene);
                if (asyncOperation is null) continue;

                asyncOperationGroup.asyncOperations.Add(asyncOperation);
                OnSceneUnLoaded.Invoke(scene);
            }

            foreach (var handle in m_AsyncOperationHandleGroup.handles)
            {
                if (handle.IsValid())
                {
                    Addressables.UnloadSceneAsync(handle);
                }
            }
            m_AsyncOperationHandleGroup.handles.Clear();

            while (!asyncOperationGroup.IsDone)
            {
                await Task.Delay(100);
            }

            if (unloadUnusedAssets)
            {
                await Resources.UnloadUnusedAssets();
            }
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> asyncOperations;
        public float Progress => asyncOperations.Count == 0 ? 0 : asyncOperations.Average(op => op.progress);
        public bool IsDone => asyncOperations.All(op => op.isDone);

        public AsyncOperationGroup(int initialCapacity)
        {
            asyncOperations = new List<AsyncOperation>(initialCapacity);
        }
    }

    public readonly struct AsyncOperationHandleGroup
    {
        public readonly List<AsyncOperationHandle<SceneInstance>> handles;
        public float Progress => handles.Count == 0 ? 0 : handles.Average(h => h.PercentComplete);
        public bool IsDone => handles.Count == 0 || handles.All(o => o.IsDone);

        public AsyncOperationHandleGroup(int initialCapacity)
        {
            handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
        }
    }
}
