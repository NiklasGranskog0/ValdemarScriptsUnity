using System;
using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
    public class PlayerStats : ScriptableObject
    {
        [Header("General Info")]
        public string heroName;
        
        [Header("Movement")]
        public float movementSpeed;
        public float sprintSpeed;
        
        [Header("Base Weapon")]
        public float initialDamage;
        public float initialAttackSpeed;
        public float initialAttackRange;
        public DamageType damageType;
        public WeaponType weaponType;

        [Header("Defense")]
        public float healthPoints;
        public ArmorType armorType;

        [Header("Stamina")]
        public StaminaStruct staminaStruct;

        [Serializable]
        public struct StaminaStruct
        {
            public float baseStamina;
            public float reduceRate;
            public float waitUntilStartRegen;
            public float regenRate;
        }
        
    }
}
