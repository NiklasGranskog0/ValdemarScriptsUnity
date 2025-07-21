using Assets.Scripts.Framework.Structs;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "GameObjectPool", menuName = "ScriptableObjects/Game Object Pool")]
    public class GameObjectPool : ScriptableObject
    {
        public PoolStruct poolStruct;
        public Transform Parent { get; private set; }

        public void CreateParent(Transform rootParent, string objName)
        {
            var gameObject = new GameObject
            {
                name = objName,
                
                transform =
                {
                    parent = rootParent,
                    position = rootParent.position,
                }
            };

            Parent = gameObject.transform;
        }
    }
}
