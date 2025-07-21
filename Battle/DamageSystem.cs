using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Battle
{
    public class DamageSystem : MonoBehaviour
    {
        public DamageInfo damageInfo;
        public DamageFormula damageFormula;

        [Header("Cooldown in seconds")]
        [SerializeField][Range(0f, 5f)]private float attackCooldown;
        public WaitForSeconds AttackCooldown { get; private set; }
        
        // Can add more stuff here, block etc.
        [Header("Values are in percent")]
        [Range(0f, 1f)]public float hitChance;
        [Range(0f, 1f)]public float dodgeChance;
        [Range(0f, 1f)]public float criticalChance;
        [Range(1f, 5f)]public float criticalModifier;

        private void Awake() => AttackCooldown = new(attackCooldown);

        public float TakeDamage(DamageSystem dmgSys)
        {
            if (dodgeChance > Random.Range(0f, 1f))
                return 0f;

            var mod = damageFormula.GetModifier(damageInfo.armorType, dmgSys.damageInfo.damageType);
            var damage = dmgSys.damageInfo.damage * mod;

            if (!(dmgSys.criticalChance > Random.Range(0f, 1f))) return damage;
            
            damage *= dmgSys.criticalModifier;
            return damage;
        }
    }
}
