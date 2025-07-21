using System.Collections.Generic;
using Assets.Scripts.Framework.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "EndlessModeSettings", menuName = "ScriptableObjects/EndlessModeSettings")]
    public class EndlessModeSettings : ScriptableObject
    {
        public ObjectEnum[] unitTypes;
        public Unit.EnemyDifficulty difficulty;
        public float SpawnRate { get; set; }
        public float maxTimeBetweenSpawns = 10f;
        public float lowestTimeBetweenSpawns = 0.5f;

        public void SetSpawnRate(float gameTime, Unit.EnemyDifficulty diff, int enemiesAlive)
        {
            SpawnRate = gameTime * enemiesAlive / (int)diff;

            // Debug.Log($"{gameTime} * {enemiesAlive} / {(int)diff} = {SpawnRate}");

            if (SpawnRate > maxTimeBetweenSpawns) SpawnRate = maxTimeBetweenSpawns;
            
            if (SpawnRate < lowestTimeBetweenSpawns && diff != Unit.EnemyDifficulty.Impossible)
                SpawnRate = lowestTimeBetweenSpawns;
        }
    }
}