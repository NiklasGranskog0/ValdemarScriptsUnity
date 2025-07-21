using UnityEngine;

namespace Assets.Scripts.Framework.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Returns object itself if it exists, null otherwise
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent<T>(out var component) ? component : gameObject.AddComponent<T>();
        }
    }
}
