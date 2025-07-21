using System;
using Assets.Scripts.Framework.Extensions;

namespace Assets.Scripts.Framework
{
    public readonly struct GenericKey : IEquatable<GenericKey>
    {
        private readonly string m_Name;
        private readonly int m_HashedKey;

        public GenericKey(string name)
        {
            m_Name = name;
            m_HashedKey = name.ComputeFNV1aHash();
        }

        public bool Equals(GenericKey other) => m_HashedKey == other.m_HashedKey;
        public override bool Equals(object obj) => obj is GenericKey other && Equals(other);
        public override int GetHashCode() => m_HashedKey;
        public override string ToString() => m_Name;

        public static bool operator ==(GenericKey left, GenericKey right) =>
            left.m_HashedKey == right.m_HashedKey;

        public static bool operator !=(GenericKey left, GenericKey right) =>
            left.m_HashedKey != right.m_HashedKey; // !(left == right)
    }

    public class GenericKey<T>
    {
        public GenericKey Key { get; }
        public T Value { get; }
        public Type ValueType { get; }
        
        public GenericKey(GenericKey key, T value)
        {
            Key = key;
            Value = value;
            ValueType = typeof(T);
        }

        public override bool Equals(object obj) => obj is GenericKey<T> other && other.Key == Key;
        public override int GetHashCode() => Key.GetHashCode();
    }
}
