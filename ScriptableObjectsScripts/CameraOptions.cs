using UnityEngine;

namespace Assets.Scripts.ScriptableObjectsScripts
{
    [CreateAssetMenu(fileName = "CameraOptions", menuName = "ScriptableObjects/CameraOptions")]
    public class CameraOptions : ScriptableObject
    {
        public float smoothTime;
    }
}
