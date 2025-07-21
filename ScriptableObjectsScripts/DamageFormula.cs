using System;
using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "DamageFormula", menuName = "ScriptableObjects/DamageFormula")]
    public class DamageFormula : ScriptableObject
    {
        [Serializable]
        public struct ComparerInternal
        {
            public ArmorType armorType;
            public float damageModifier;
        }
        
        [Serializable]
        public struct ModifierComparer
        {
            public DamageType damageType;
            public ComparerInternal[] modiferAgainstArmorType;
        }

        public ModifierComparer[] damageFormula;

        public float GetModifier(ArmorType armorType, DamageType damageType)
        {
            var index = (int)damageType;
            Span<ComparerInternal> span = damageFormula[index].modiferAgainstArmorType;

            for (var i = 0; i < span.Length; i++)
            {
                if (span[i].armorType == armorType)
                    return span[i].damageModifier;
            }

            return 0f;
        }
    }
}
