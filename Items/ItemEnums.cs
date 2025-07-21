using System;

namespace Assets.Scripts.Items
{
    public static class ItemEnums
    {
        public enum ItemSlot
        {
            None, Head, Neck, Shoulders, Back, Chest, Shirt, Tabard, Wrist,
            Hands, Waist, Legs, Feet, Finger1, Finger2, Trinket1, Trinket2,
            MainHand, OffHand, OneHand, TwoHanded, Ranged, Relic,
        }
        
        public static string WeaponItemSlotToString(this ItemSlot weaponType)
        {
            return weaponType switch
            {
                ItemSlot.None => string.Empty,
                ItemSlot.TwoHanded => "Two-Hand",
                ItemSlot.MainHand => "Main Hand",
                ItemSlot.OffHand => "Off Hand",
                ItemSlot.OneHand => "One-Hand",
                ItemSlot.Ranged => "Ranged",
                ItemSlot.Relic => "Relic",
                _ => throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null)
            };
        }

        public enum WeaponType
        {
            None,
            Sword,
            Axe,
            Shield,
        }

        public enum RangedWeaponType
        {
            None,
            Bow,
            Crossbow,
            Gun,
            Wand,
        }

        public enum RangedWeaponAmmoType
        {
            None,
            Arrows,
            Bullets,
            Magic,
        }
    }
}
