using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Framework.SceneManagement
{
    [CreateAssetMenu(fileName = "SceneGroupAsset", menuName = "ScriptableObjects/SceneGroupAsset")]
    public class SceneGroupAsset : ScriptableObject
    {
        public List<SceneGroup> sceneGroups;
    }
}