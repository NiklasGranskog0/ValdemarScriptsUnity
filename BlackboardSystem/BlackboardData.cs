using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Framework.BlackboardSystem
{
    [CreateAssetMenu(fileName = "New Blackboard Data", menuName = "ScriptableObjects/Blackboard Data")]
    public class BlackboardData : ScriptableObject
    {
        public List<BlackboardEntryData> entries = new();

        public void SetValuesOnBlackboard(Blackboard blackboard)
        {
            foreach (var entry in entries)
            {
                entry.SetValueOnBlackboard(blackboard);
            }
        }
    }

    [Serializable]
    public class BlackboardEntryData : ISerializationCallbackReceiver
    {
        public string keyName;
        public AnyValue.ValueType valueType;
        public AnyValue value;

        public void SetValueOnBlackboard(Blackboard blackboard)
        {
            var key = blackboard.GetOrRegisterKey(keyName);
            s_setValueDispatchTable[value.type](blackboard, key, value);
        }
        
        // Dispatch table to set different types of value on the blackboard
        private static Dictionary<AnyValue.ValueType, Action<Blackboard, BlackboardKey, AnyValue>>
            s_setValueDispatchTable = new()
            {
                { AnyValue.ValueType.Bool, (blackboard, key, anyValue) => blackboard.SetValue<bool>(key, anyValue) },
                { AnyValue.ValueType.Int, (blackboard, key, anyValue) => blackboard.SetValue<int>(key, anyValue) },
                { AnyValue.ValueType.Float, (blackboard, key, anyValue) => blackboard.SetValue<float>(key, anyValue) },
                { AnyValue.ValueType.String, (blackboard, key, anyValue) => blackboard.SetValue<string>(key, anyValue) },
                { AnyValue.ValueType.Vector3, (blackboard, key, anyValue) => blackboard.SetValue<Vector3>(key, anyValue) },
                { AnyValue.ValueType.Transform, (blackboard, key, anyValue) => blackboard.SetValue<Transform>(key, anyValue) },
                { AnyValue.ValueType.Object, (blackboard, key, anyValue) => blackboard.SetValue<UnityEngine.Object>(key, anyValue) },
            };

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => value.type = valueType;
    }

    [Serializable]
    public struct AnyValue
    {
        public enum ValueType { Int, Float, Bool, String, Vector3, Transform, Object }
        public ValueType type;
        
        // Storage for different types of values
        public bool boolValue;
        public int intValue;
        public float floatValue;
        public string stringValue;
        public Vector3 vector3Value;
        public Transform transformValue;
        public UnityEngine.Object objectValue;
        
        // Implicit conversion operators to convert AnyValue to different types
        public static implicit operator bool(AnyValue value) => value.ConvertValue<bool>();
        public static implicit operator int(AnyValue value) => value.ConvertValue<int>();
        public static implicit operator float(AnyValue value) => value.ConvertValue<float>();
        public static implicit operator string(AnyValue value) => value.ConvertValue<string>();
        public static implicit operator Vector3(AnyValue value) => value.ConvertValue<Vector3>();
        public static implicit operator Transform(AnyValue value) => value.ConvertValue<Transform>();
        public static implicit operator UnityEngine.Object(AnyValue value) => value.ConvertValue<UnityEngine.Object>();

        private T ConvertValue<T>()
        {
            return type switch
            {
                ValueType.Bool => AsBool<T>(boolValue),
                ValueType.Int => AsInt<T>(intValue),
                ValueType.Float => AsFloat<T>(floatValue),
                ValueType.String => (T)(object)stringValue,
                ValueType.Vector3 => (T)(object)vector3Value,
                ValueType.Transform => AsTransform<T>(transformValue), // ?
                ValueType.Object => (T)(object)objectValue,
                _ => throw new NotSupportedException($"Not supported value type: {typeof(T)}")
            };
        }
        
        // Methods to convert primitive types to generic types with type safety and without boxing
        private T AsBool<T>(bool value) => typeof(T) == typeof(bool) && value is T correctType ? correctType : default;
        private T AsInt<T>(int value) => typeof(T) == typeof(int) && value is T correctType ? correctType : default;
        private T AsFloat<T>(float value) => typeof(T) == typeof(float) && value is T correctType ? correctType : default;
        private T AsTransform<T>(Transform value) => typeof(T) == typeof(Transform) && value is T correctType ? correctType : default;
    }
}