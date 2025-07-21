using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework;
using Assets.Scripts.Framework.Enums;
using Assets.Scripts.Framework.ServiceManagement;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameObjectPooler : MonoBehaviour
    {
        [SerializeField] private List<GameObjectPool> gameObjectPools;
        private readonly Dictionary<ObjectEnum, ObjectPool> m_PoolDictionary = new();

        private void Awake()
        {
            ServiceLocator.ForSceneOf(this).Register(this, ServiceLevel.Scene, gameObject.scene.name);

            CreatePools();
        }

        public GameObject GetFromPool(ObjectEnum obj, bool rent = true)
        {
            return m_PoolDictionary.ContainsKey(obj) ? GetPool(obj).Rent(rent) : null;
        }

        public GameObject GetFromPool(Unit.UnitType type, bool rent = true)
        {
            var obj = type.UnitToObjectEnum();
            return GetFromPool(obj, rent);
        }

        private void CreatePools()
        {
            foreach (var pool in gameObjectPools)
            {
                pool.CreateParent(gameObject.transform, pool.name);
                
                m_PoolDictionary.Add(pool.poolStruct.objectEnum,
                    new ObjectPool(pool.poolStruct.initialQuantity, pool.poolStruct.gameObject, pool.Parent));
            }
        }

        private ObjectPool GetPool(ObjectEnum enumKey)
        {
            foreach (var objPool in m_PoolDictionary.Where(objPool => objPool.Key.Equals(enumKey)))
            {
                return objPool.Value;
            }

            throw new Exception($"Pool with the key '{enumKey}' does not exists!");
        }
    }
}