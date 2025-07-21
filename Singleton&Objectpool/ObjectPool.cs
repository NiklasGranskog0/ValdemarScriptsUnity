using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Framework
{
    public class ObjectPool
    {
        private readonly uint m_ExpandBy;
        private readonly GameObject m_Prefab;
        private readonly Transform m_Parent;
        private readonly Stack<GameObject> m_Objects = new();

        public ObjectPool(uint initSize, GameObject prefab, Transform parent = null, uint expandBy = 1)
        {
            m_ExpandBy = expandBy < 1 ? 1 : expandBy;
            m_Prefab = prefab;
            m_Parent = parent;
            Expand(initSize < 1 ? 1 : initSize);
        }
        
        private void Expand(uint amount)
        {
            for (var i = 0; i < amount; i++)
            {
                var instance = Object.Instantiate(m_Prefab, m_Parent);
                instance.SetActive(false);

                var emitOnDisable = instance.AddComponent<EmitOnDisable>();
                emitOnDisable.OnDisableGameObject += UnRent;
                m_Objects.Push(instance);
            }
        }
        
        private void UnRent(GameObject gameObject)
        {
            m_Objects.Push(gameObject);
        }

        public GameObject Rent(bool activate)
        {
            if (m_Objects.Count == 0)
            {
                Expand(m_ExpandBy);
            }

            var instance = m_Objects.Pop();
            instance = instance != null ? instance : Rent(activate);
            instance.SetActive(activate);
            return instance;
        }
    }
}
