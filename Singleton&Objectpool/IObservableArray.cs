using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Framework
{
    public interface IObservableArray<T>
    {
        event Action<T[]> AnyValueChanged;
    
        int Count { get; }
        T this[int index] { get; }

        void Swap(int index1, int index2);
        void Clear();
        bool TryAdd(T item);
        bool TryRemove(T item);
    }

    [Serializable]
    public class ObservableArray<T> : IObservableArray<T>
    {
        private T[] m_Items;
        
        public event Action<T[]> AnyValueChanged = delegate {  };
        public int Count => m_Items.Count(i => i != null);
        public T this[int index] => m_Items[index];

        public ObservableArray(int size = 20, IList<T> initialList = null)
        {
            m_Items = new T[size];
            if (initialList != null)
            {
                initialList.Take(size).ToArray().CopyTo(m_Items, 0);
                Invoke();
            }
        }

        void Invoke() => AnyValueChanged.Invoke(m_Items);

        public void Swap(int index1, int index2)
        {
            (m_Items[index1], m_Items[index2]) = (m_Items[index2], m_Items[index1]);
            Invoke();
        }

        public void Clear()
        {
            m_Items = new T[m_Items.Length];
        }

        public bool TryAdd(T item)
        {
            for (int i = 0; i < m_Items.Length; i++)
            {
                if (m_Items[i] != null) continue;
                m_Items[i] = item;
                Invoke();
                return true;
            }

            return false;
        }

        public bool TryRemove(T item)
        {
            for (int i = 0; i < m_Items.Length; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(m_Items[i], item)) continue;
                m_Items[i] = default;
                Invoke();
                return true;
            }

            return false;
        }
    }
}