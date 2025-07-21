using System.Collections.Generic;
using Assets.Scripts.Framework.Enums;

namespace Assets.Scripts.Framework.Extensions
{
    public static class ItemRarityColors
    {
        public static string SetColorByRarity(this string s, ItemRarity rarity)
        {
            var color = s_colorDictionary[rarity];
            
            return $"<color={color}>{s}</color>";
        }
        
        private static readonly Dictionary<ItemRarity, string> s_colorDictionary = new()
        {
            { ItemRarity.Common, "white"},
            { ItemRarity.Uncommon, "green"},
            { ItemRarity.Rare, "blue"},
            { ItemRarity.Epic, "purple"},
            { ItemRarity.Legendary, "orange"},
        };
    }
}
