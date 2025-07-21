using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "Drop Table", menuName = "ScriptableObjects/New Drop Table")]
    public class DropTable : ScriptableObject
    {
        public List<ItemData> droppableItems;
        [Header("Drop chance in percent (%)")]
        [SerializeField] private int chanceToDropItem;
        public float ChanceToDropItem => chanceToDropItem * 0.01f;
    }
}
