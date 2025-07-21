using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.Framework.Extensions;
using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Framework.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        public enum SceneGroupToLoad
        {
            MainMenu,
            VisbyFields,
            Test,
            InventoryTest,
            Bootstrapper,
        }

        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider progressBar;
        [SerializeField] private SceneGroupAsset sceneGroupAssets;
        [SerializeField] private float fillSpeed;
        [SerializeField] private SceneGroupToLoad sceneGroupToLoad;

        private const float k_TargetProgress = 1f;
        public bool isLoading;

        public SoundData soundData;

        private readonly SceneGroupManager m_Manager = new();
        private readonly LoadingProgress m_LoadingProgress = new();

        // Awake will run twice because it lives in the Bootstrapper scene (IF Starting in Bootstrapper Scene)
        private void Awake()
        {
            // TODO: Use event for something useful 
#if UNITY_EDITOR
            m_Manager.OnSceneLoaded += sceneName => Debug.Log("SceneLoader: ".Color("red") + $"Loaded: {sceneName}".Color("lightblue"));
            m_Manager.OnSceneUnLoaded += sceneName => Debug.Log("SceneLoader: ".Color("red") + $"UnLoaded: {sceneName}".Color("lightblue"));
#endif
            m_Manager.OnSceneGroupLoaded += FinishedLoading;
        }

        private async void Start()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);

            m_LoadingProgress.Progressed += ProgressReport;
            await LoadSceneGroupByIndex((int)sceneGroupToLoad);
        }

        private void Update()
        {
            if (!isLoading) return;

            var currentFillAmount = progressBar.value;
            progressBar.value = Mathf.Lerp(currentFillAmount, k_TargetProgress, Time.deltaTime * fillSpeed);
        }

        public async Task LoadSceneGroupByIndex(int index)
        {
            progressBar.value = 0f;

#if UNITY_EDITOR
            Debug.Log("SceneLoader: ".Color("red") + $"Loading scene group {(SceneGroupToLoad)index}".Color("lightblue"));

            if (index < 0 || index >= sceneGroupAssets.sceneGroups.Count)
            {
                Debug.LogError($"Invalid scene group index: {index}");
            }
#endif
            EnableLoadingCanvas();
            await m_Manager.LoadScenes(sceneGroupAssets.sceneGroups[index], m_LoadingProgress);
        }

        public async Task LoadSceneGroupByName(string groupName)
        {
            progressBar.value = 0f;

            foreach (var sceneGroup in sceneGroupAssets.sceneGroups.Where(sceneGroup => groupName.Equals(sceneGroup.groupName)))
            {
                EnableLoadingCanvas();
                await m_Manager.LoadScenes(sceneGroup, m_LoadingProgress);
            }
        }

        private void ProgressReport(float value)
        {
        }

        private void FinishedLoading()
        {
            EnableLoadingCanvas(false);
            SoundManager.Singleton.PlaySound(soundData);
#if UNITY_EDITOR
            Debug.Log("SceneLoader: ".Color("red") + "Finished Loading Scene Group".Color("lightblue"));
#endif
        }

        private void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingScreen.SetActive(enable);
        }
    }
}