using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObjects/EnemyStats")]
    public class EnemyStats : ScriptableObject
    {
        [Header("Movement")]
        public float movementSpeed;
        
        [Header("Base Weapon")]
        public float initialDamage;
        public float attackCooldown;
        public DamageType damageType;
        public WeaponType weaponType;

        [Header("Defense")]
        public float healthPoints;
        public ArmorType armorType;
    }
}