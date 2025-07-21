using System;
using Assets.Scripts.Framework.Enums;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    [Serializable]
    public class DamageInfo
    {
        [HideInInspector] public float damage;
        public ArmorType armorType;
        public DamageType damageType;
    }
}
