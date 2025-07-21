using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "PlayerValues", menuName = "ScriptableObjects/PlayerValues")]
    public class PlayerValues : ScriptableObject
    {
        [Tooltip("Increase attack power")]
        public int strength;

        [Tooltip("Increase movement speed & dodge")]
        public int agility;
        
        [Tooltip("Increase mana & spell damage")]
        public int intellect;
        
        [Tooltip("Increase health & stamina(sprint) regen")]
        public int spirit;
        
        [Tooltip("Increase amount of health")]
        public int stamina;
        
        [Tooltip("Decrease damage taken")]
        public int armor;
    }
}
