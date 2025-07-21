using System;
using System.Collections;
using Assets.Scripts.Framework.Enums;

namespace Assets.Scripts.Interfaces
{
    public interface ISpawner
    {
        public event Action EnemyKilledEvent;
        public void InvokeEnemyKilled();
        
        public bool IsWaveCleared();
        public void SpawnWave();
        public UnityEngine.Vector3 RandomSpawnPosition();
        public void SpawnBossEnemy(Unit.BossType type);
    }
}
