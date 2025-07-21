using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "EnemyWaveData", menuName = "ScriptableObjects/EnemyWaveData")]
    public class EnemyWaveData : ScriptableObject
    {
        public Wave wave;
       
        [System.Serializable]
        public struct Wave
        {
            public UnitTypeAndAmount unitTypeAndAmount;
            public Unit.EnemyStrengthMultiplier strengthMultiplier;

            public bool endWaveWithBoss;
            public Unit.BossType endWaveBoss;
            [HideInInspector] public bool waveDead;
            [HideInInspector] public bool bossDead;
        }
        
        [System.Serializable]
        public struct UnitTypeAndAmount
        {
            public Unit.UnitType[] unitTypes;
            
            [Range(1, 100000)]
            public int[] amounts;
        }
    }
}