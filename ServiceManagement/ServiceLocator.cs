using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Framework.ServiceManagement
{
    public enum ServiceLevel
    {
        Global,
        Scene,
        Local,
    }
    
    public class ServiceLocator : MonoBehaviour
    {
        private static ServiceLocator s_global;
        private static Dictionary<Scene, ServiceLocator> s_sceneContainers;
        private readonly ServiceManager m_Services = new();

        private const string k_GlobalServiceLocatorName = "ServiceLocator [Global]";
        private const string k_SceneServiceLocatorName = "ServiceLocator [Scene]";

        private static List<GameObject> s_temporarySceneGameObjects;

        internal void ConfigureAsGlobal()
        {
            if (s_global == this) {
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
            } else if (s_global != null) {
                Debug.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
            } else {
                s_global = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        internal void ConfigureForScene()
        {
            var scene = gameObject.scene;

            if (s_sceneContainers.ContainsKey(scene))
            {
                Debug.LogError(
                    "ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene!",
                    this);
                return;
            }

            s_sceneContainers.Add(scene, this);
        }

        public static ServiceLocator Global
        {
            get
            {
                if (s_global != null) return s_global;

                if (FindFirstObjectByType<ServiceLocatorGlobal>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return s_global;
                }

                var container = new GameObject(k_GlobalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();

                return s_global;
            }
        }

        public static ServiceLocator For(MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(monoBehaviour) ?? Global;
        }

        public static ServiceLocator ForSceneOf(MonoBehaviour monoBehaviour)
        {
            var scene = monoBehaviour.gameObject.scene;

            if (s_sceneContainers.TryGetValue(scene, out var container) && container != monoBehaviour)
                return container;

            s_temporarySceneGameObjects.Clear();
            scene.GetRootGameObjects(s_temporarySceneGameObjects);

            foreach (var obj in s_temporarySceneGameObjects.Where(obj =>
                         obj.GetComponent<ServiceLocatorScene>() != null))
            {
                if (obj.TryGetComponent<ServiceLocatorScene>(out var bootstrapper)
                    && bootstrapper.Container != monoBehaviour)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return s_global;
        }

        public ServiceLocator Register<T>(T service, ServiceLevel level, string sceneName = "")
        {
            m_Services.Register(service);
            LogRegisterService(service, level, sceneName);
            return this;
        }

        public ServiceLocator Register(Type type, object service, ServiceLevel level, string sceneName = "")
        {
            m_Services.Register(type, service);
            LogRegisterService(service, level, sceneName);
            return this;
        }

        private void LogRegisterService<T>(T service, ServiceLevel level, string sceneName = "")
        {
#if UNITY_EDITOR
            var endString = $"Registered service of the type {typeof(T).Name.Color("red")}".Color("lightblue");
            var globalString = "ServiceLocator ".Color("red") + $"[{level.ToString()}]".Color("orange") + ": ".Color("red");
            var localOrScene = "ServiceLocator ".Color("red") + $"[{level + " | " + sceneName}]".Color("orange") + ": ".Color("red");
            
            switch (level)
            {
                case ServiceLevel.Global:
                    Debug.Log(globalString + endString);
                    break;
                case ServiceLevel.Scene:
                    Debug.Log(localOrScene + endString);
                    break;
                case ServiceLevel.Local:
                    Debug.Log(localOrScene + endString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
#endif
        }

        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if (TryGetNextInHierarchy(out var container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException("ServiceLocator: ".Color("red") + $"Service of type {typeof(T).Name.Color("red")} not registered!".Color("lightblue"));
        }

        private bool TryGetService<T>(out T service) where T : class
        {
            return m_Services.TryGet(out service);
        }
        
        public T TryGet<T>() where T : class
        {
            m_Services.TryGet<T>(out var service);
            return service;
        }

        private bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == s_global)
            {
                container = null;
                return false;
            }

            container = transform.parent.OrNull()?.GetComponentInParent<ServiceLocator>().OrNull() ?? ForSceneOf(this);
            return container != null;
        }

        private void OnDestroy()
        {
            if (this == s_global)
                s_global = null;
            else if (s_sceneContainers.ContainsValue(this))
                s_sceneContainers.Remove(gameObject.scene);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_global = null;
            s_sceneContainers = new();
            s_temporarySceneGameObjects = new();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        private static void AddGlobal()
        {
            var obj = new GameObject(k_GlobalServiceLocatorName, typeof(ServiceLocatorGlobal));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        private static void AddScene()
        {
            var obj = new GameObject(k_SceneServiceLocatorName, typeof(ServiceLocatorScene));
        }
#endif
    }
}