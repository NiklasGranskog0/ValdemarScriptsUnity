using System;
using UnityEngine;

namespace Assets.Scripts.Framework.Extensions
{
    [Serializable]
    public struct SerializableGuid : IEquatable<SerializableGuid>
    {
        [SerializeField, HideInInspector] public uint part1;
        [SerializeField, HideInInspector] public uint part2;
        [SerializeField, HideInInspector] public uint part3;
        [SerializeField, HideInInspector] public uint part4;

        public SerializableGuid(uint value1, uint value2, uint value3, uint value4)
        {
            part1 = value1;
            part2 = value2;
            part3 = value3;
            part4 = value4;
        }

        public SerializableGuid(Guid guid)
        {
            var bytes = guid.ToByteArray();
            part1 = BitConverter.ToUInt32(bytes, 0);
            part2 = BitConverter.ToUInt32(bytes, 4);
            part3 = BitConverter.ToUInt32(bytes, 8);
            part4 = BitConverter.ToUInt32(bytes, 12);
        }

        public static SerializableGuid FromHexString(string hexString)
        {
            if (hexString.Length != 32) return Empty;

            return new SerializableGuid
            (
                Convert.ToUInt32(hexString.Substring(0, 8), 16),
                Convert.ToUInt32(hexString.Substring(8, 8), 16),
                Convert.ToUInt32(hexString.Substring(16, 8), 16),
                Convert.ToUInt32(hexString.Substring(24, 8), 16)
            );
        }

        public Guid ToGuid()
        {
            var bytes = new Byte[16];
            BitConverter.GetBytes(part1).CopyTo(bytes, 0);
            BitConverter.GetBytes(part2).CopyTo(bytes, 4);
            BitConverter.GetBytes(part3).CopyTo(bytes, 8);
            BitConverter.GetBytes(part4).CopyTo(bytes, 12);
            return new Guid(bytes);
        }

        public string ToHexString()
        {
            return $"{part1:X8}{part2:X8}{part3:X8}{part4:X8}";
        }
        
        public static SerializableGuid Empty => new(0, 0, 0, 0);
        public static SerializableGuid NewGuid => Guid.NewGuid().ToSerializableGuid();

        public static implicit operator Guid(SerializableGuid serializableGuid) => serializableGuid.ToGuid();
        public static implicit operator SerializableGuid(Guid guid) => new SerializableGuid(guid);
        public static bool operator ==(SerializableGuid left, SerializableGuid right) => left.Equals(right);
        public static bool operator !=(SerializableGuid left, SerializableGuid right) => !(left == right);

        public override bool Equals(object obj)
        {
            return obj is SerializableGuid guid && Equals(guid);
        }

        public bool Equals(SerializableGuid other)
        {
            return part1 == other.part1 && part2 == other.part2 && part3 == other.part3 && part4 == other.part4;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(part1, part2, part3, part4);
        }
    }
}
