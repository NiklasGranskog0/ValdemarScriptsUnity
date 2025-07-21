namespace Assets.Scripts.Framework.Enums
{
    public class Enums
    {
        
    }

    #region Combat

    public enum WeaponType
    {
        SwordAndShield,
        RollingPin,
        Crossbow,
        Sword,
        Spear,
        Knife,
        Axe,
        Bow,
    }
        
    public enum DamageType
    {
        Piercing,
        Normal,
        Blunt,
        Splash,
    }

    public enum ArmorType
    {
        Light,
        Medium,
        Heavy,
    }

    #endregion

    #region GameState

    public enum State
    {
        Pause,
        UnPause,
        Gameplay,
        LevelUp,
        Looting,
        Menu,
        Exit,
        Win,
        Lose,
    }

    #endregion

    #region Objects

    public enum ObjectEnum
    {
        ExpObject,
        FootSoldier,
        Archer,
        ItemDrop,
        SoundEmitter,
    }

    #endregion

    #region Items

    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    #endregion
}