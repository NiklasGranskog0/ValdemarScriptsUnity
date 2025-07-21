using System.Collections.Generic;
using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "UnitByEnum", menuName = "ScriptableObjects/UnitByEnum")]
    public class UnitByEnum : ScriptableObject
    {
        [System.Serializable]
        public struct UnitByType 
        {
            public GameObject unit;
            public Unit.UnitType unitType;
        }

        public UnitByType[] unitByLevels;
        private readonly Dictionary<Unit.UnitType, GameObject> m_Dictionary = new();

        public GameObject this[Unit.UnitType level]
        {
            get
            {
                Init();
                return m_Dictionary[level];
            }
        }

        private void Init()
        {
            foreach (var unit in unitByLevels)
            {
                m_Dictionary[unit.unitType] = unit.unit;
            }
        }
    }
}
