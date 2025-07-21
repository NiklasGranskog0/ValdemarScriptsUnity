using System;

namespace Assets.Scripts.Framework.Enums
{
    public static class Unit
    {
        public static ObjectEnum UnitToObjectEnum(this UnitType type)
        {
            return type switch
            {
                UnitType.FootSoldier => ObjectEnum.FootSoldier,
                UnitType.Archer => ObjectEnum.Archer,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        public enum UnitType
        {
            FootSoldier, Archer, 
        }

        public enum BossType
        {
            ErikAvSachsen,
            KristofferAvLolland,
            ValdemarAtterdag,
        }
            
        public enum EnemyStrengthMultiplier
        {
            X1 = 1, X2, X3, X4, X5, X6, X7, X8, X9, X10
        }

        public enum EnemyDifficulty
        {
            VeryEasy = 1,
            Easy = 5,
            Medium = 25,
            Hard = 50,
            VeryHard = 65,
            Insane = 80,
            Impossible = 100, 
        }
    }
}
