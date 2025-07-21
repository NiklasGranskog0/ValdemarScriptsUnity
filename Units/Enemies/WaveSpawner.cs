using System;
using System.Collections;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.Extensions;
using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Units.Enemies
{
    // TODO: Want to make a universal spawner for Timed/Endless/Etc.. 
    
    public class WaveSpawner : MonoBehaviour
    {
        // TODO: Single WaveSpawner for Endless, Timed, WaveMode
        
        public EnemyWaveData[] enemyWaveData;
        public Transform[] spawnPoints;
        private GameObjectPooler m_GameObjectPooler;

        private int m_NumberOfKilledEnemies;
        private int m_NumberOfEnemiesInWave;
        private int m_WaveIndex;
        private event Action EnemyKilledEvent = delegate { };
        public int GetWaveStrength => (int)enemyWaveData[m_WaveIndex].wave.strengthMultiplier;

        private void Start()
        {
            m_GameObjectPooler = ServiceLocator.ForSceneOf(this).TryGet<GameObjectPooler>();
        }

        public void InvokeEnemyKilled()
        {
            m_NumberOfKilledEnemies++;
            Debug.Log($"IsWaveCleared: killed: {m_NumberOfKilledEnemies} | Needed to clear: {m_NumberOfEnemiesInWave}");
            if (m_NumberOfKilledEnemies < m_NumberOfEnemiesInWave) return;
            EnemyKilledEvent.Invoke();
        }

        private void IsWaveCleared()
        {
            if (m_WaveIndex + 1 >= enemyWaveData.Length) return;

            enemyWaveData[m_WaveIndex].wave.waveDead = true;

            if (enemyWaveData[m_WaveIndex].wave.endWaveWithBoss)
            {
            }
            
            m_WaveIndex++;
            UpdateEnemyList(m_WaveIndex); // TEMP

            if (!enemyWaveData[m_WaveIndex - 1].wave.endWaveWithBoss)
            {
                SpawnWave();
            }
        }

        private void SpawnWave()
        {
            // Cache data
            
            var length = enemyWaveData[m_WaveIndex].wave.unitTypeAndAmount.unitTypes.Length;

            // These should be the same length
            var amountsLength = enemyWaveData[m_WaveIndex].wave.unitTypeAndAmount.amounts;
            var typeLength = enemyWaveData[m_WaveIndex].wave.unitTypeAndAmount.unitTypes;

            for (int i = 0; i < length; i++)
            {
                // SpawnUnit(typeLength[i], amountsLength[i]);
                StartCoroutine(SpawnUnits(typeLength[i], amountsLength[i], 2f));
            }
        }

        private Vector3 RandomPosition()
        {
            while (true)
            {
                var random = Random.Range(0, spawnPoints.Length);
                var trySpawnPoint = spawnPoints[random].gameObject.activeInHierarchy;

                if (!trySpawnPoint) Debug.Log("Get New Position".Color("red"));
                
                if (trySpawnPoint)
                    return spawnPoints[random].position;
            }
        }

        private void UpdateEnemyList(int index)
        {
            m_NumberOfEnemiesInWave = 0;
            m_NumberOfKilledEnemies = 0;

            foreach (var amount in enemyWaveData[index].wave.unitTypeAndAmount.amounts)
            {
                m_NumberOfEnemiesInWave += amount;
            }
        }

        private IEnumerator SpawnUnits(Unit.UnitType type, int amountToSpawn, float timeBetweenSpawns)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                var unActiveUnit = m_GameObjectPooler.GetFromPool(type, false);
                unActiveUnit.transform.position = RandomPosition();
                unActiveUnit.SetActive(true);
                
                // If wait for seconds here will be time between each spawn
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
            
            // Time between group of spawns
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}