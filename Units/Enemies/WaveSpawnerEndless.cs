using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using Assets.Scripts.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Units.Enemies
{
    public class WaveSpawnerEndless : MonoBehaviour
    {
        [SerializeField] private EndlessModeSettings endlessSettings;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private bool endlessMode;
        [SerializeField] private bool timedMode;

        private GameTimer m_GameTimer;
        private GameObjectPooler m_GameObjectPooler;
        private EnemyHandler m_EnemyHandler;

        // TEMP
        private float m_Time;
        private bool m_Start;

        private void Start()
        {
            m_GameTimer = ServiceLocator.Global.TryGet<GameTimer>();
            m_GameObjectPooler = ServiceLocator.ForSceneOf(this).TryGet<GameObjectPooler>();
            m_EnemyHandler = ServiceLocator.Global.TryGet<EnemyHandler>();

            if (endlessMode)
            {
                EndlessModeSettings();
            }
            
            // SpawnEnemies(2);
        }

        private void EndlessModeSettings()
        {
            m_GameTimer.EndlessMode = true;
            endlessSettings.SetSpawnRate(m_GameTimer.GetCurrentEndlessTimeSeconds, endlessSettings.difficulty,
                m_EnemyHandler.GetEnemyCount);
            
            m_Start = true;
        }

        private void Update()
        {
            if (!m_Start) return;
            
            m_Time += Time.deltaTime;
            
            if (m_Time >= endlessSettings.SpawnRate)
            {
                SpawnEnemies(1);
                m_Time = 0f;
            }
        }

        private Vector3 RandomSpawnPosition()
        {
            while (true)
            {
                var random = Random.Range(0, spawnPoints.Length);
                var trySpawnPoint = spawnPoints[random].gameObject.activeInHierarchy;

                if (trySpawnPoint)
                    return spawnPoints[random].position;
            }
        }

        private void SpawnEnemies(int amountToSpawn)
        {
            var random = Random.Range(0, endlessSettings.unitTypes.Length);
            var type = endlessSettings.unitTypes[random];

            for (int i = 0; i < amountToSpawn; i++)
            {
                var unActiveUnit = m_GameObjectPooler.GetFromPool(type, false);
                unActiveUnit.transform.position = RandomSpawnPosition();
                unActiveUnit.SetActive(true);
                m_EnemyHandler.AddEnemy(unActiveUnit.GetComponent<EnemyBase>());
            }

            endlessSettings.SetSpawnRate(m_GameTimer.GetCurrentEndlessTimeSeconds, endlessSettings.difficulty,
                m_EnemyHandler.GetEnemyCount);
        }
    }
}