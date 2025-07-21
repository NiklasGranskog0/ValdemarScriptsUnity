using System;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;

namespace Assets.Scripts.Framework.SceneManagement
{
    [Serializable]
    public class SceneGroup
    {
        public string groupName = "New Scene Group";
        public List<SceneData> scenes;

        public string FindSceneByType(SceneType sceneType)
        {
            return scenes.FirstOrDefault(scene => scene.sceneType == sceneType).Name;
        }
    }

    [Serializable]
    public struct SceneData
    {
        public SceneReference reference;
        public string Name => reference.Name;
        public SceneType sceneType;
    }

    public enum SceneType
    {
        ActiveScene,
        MainMenu,
        UserInterface,
        GameHUD,
        Cinematic,
        Environment
        // etc
    }
}
