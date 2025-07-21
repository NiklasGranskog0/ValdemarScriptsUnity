using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "New_Droppable_ItemValues", menuName = "ScriptableObjects/Items/Create New Item Droppable Values")]
    public class DroppableItemValues : ScriptableObject
    {
        public ItemEnums.ItemSlot itemSlot;
        public ItemEnums.WeaponType weaponType;
        
        [Space]
        public int intellect;
        public int strength;
        public int stamina;
        public int agility;
        public int spirit;
        public int armor;
        
        [Header("Extra values if it is a Weapon")]
        public float speed;
        public string damage;
        public float damagePerSecond;
    }
}
