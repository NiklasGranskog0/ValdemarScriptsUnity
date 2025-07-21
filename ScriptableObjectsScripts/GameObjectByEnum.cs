using System.Collections.Generic;
using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "GameObjectByEnum", menuName = "ScriptableObjects/GameObjectByEnum")]
    public class GameObjectByEnum : ScriptableObject
    {
        [System.Serializable]
        public struct GameObjectByType
        {
            public GameObject @object;
            public ObjectEnum @enum;
        }

        public GameObjectByType[] objectByType;
        private readonly Dictionary<ObjectEnum, GameObject> m_Dictionary;

        public GameObject this[ObjectEnum objectEnum]
        {
            get
            {
                Init();
                return m_Dictionary[objectEnum];
            }
        }

        private void Init()
        {
            foreach (var obj in objectByType)
            {
                m_Dictionary[obj.@enum] = obj.@object;
            }
        }
    }
}
