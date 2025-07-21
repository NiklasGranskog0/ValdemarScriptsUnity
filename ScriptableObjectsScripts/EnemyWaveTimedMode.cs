using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "EnemyWaveTimedMode", menuName = "ScriptableObjects/EnemyWaveTimedMode")]
    public class EnemyWaveTimedMode : ScriptableObject
    {
        public WaveTimedMode timedWave;
        
        [System.Serializable]
        public struct WaveTimedMode
        {
            public Unit.UnitType[] unitTypes;
            public float time;
            public float frequencyOfSpawns;
            public Unit.EnemyStrengthMultiplier strengthMultiplier;
            
            public Unit.BossType[] bossTypes;
        }
    }
}
