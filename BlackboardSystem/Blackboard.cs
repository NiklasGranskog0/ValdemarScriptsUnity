using System;
using System.Collections.Generic;
using Assets.Scripts.Framework.Extensions;

namespace Assets.Scripts.Framework.BlackboardSystem
{
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        private readonly string m_Name;
        private readonly int m_HashedKey;

        public BlackboardKey(string name)
        {
            m_Name = name;
            m_HashedKey = name.ComputeFNV1aHash();
        }

        public bool Equals(BlackboardKey other) => m_HashedKey == other.m_HashedKey;
        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => m_HashedKey;
        public override string ToString() => m_Name;

        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs.m_HashedKey == rhs.m_HashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
    }

    [Serializable]
    public class BlackboardEntry<T>
    {
        public BlackboardKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }

        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }

        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
        public override int GetHashCode() => Key.GetHashCode();
    }

    [Serializable]
    public class Blackboard
    {
        private Dictionary<string, BlackboardKey> m_KeyRegistry = new();
        private Dictionary<BlackboardKey, object> m_Entries = new();

        public List<Action> PassedActions { get; } = new();

        public void AddAction(Action action)
        {
            Preconditions.CheckNotNull(action);
            PassedActions.Add(action);
        }

        public void ClearActions() => PassedActions.Clear();

        public void Debug()
        {
            foreach (var entry in m_Entries)
            {
                var entryType = entry.Value.GetType();

                if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
                {
                    var valueProperty = entryType.GetProperty("Value");
                    if (valueProperty == null) continue;
                    var value = valueProperty.GetValue(entry.Value);
                    UnityEngine.Debug.Log($"Key: {entry.Key}, Value: {value}");
                }
            }
        }
        
        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            if (m_Entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue<T>(BlackboardKey key, T value)
        {
            m_Entries[key] = new BlackboardEntry<T>(key, value);
        }
        
        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            Preconditions.CheckNotNull(keyName);

            if (!m_KeyRegistry.TryGetValue(keyName, out var key))
            {
                key = new BlackboardKey(keyName);
                m_KeyRegistry[keyName] = key;
            }

            return key;
        }

        public bool ContainsKey(BlackboardKey key) => m_Entries.ContainsKey(key);
        public void Remove(BlackboardKey key) => m_Entries.Remove(key);
    }
}
