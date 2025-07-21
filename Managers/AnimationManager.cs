using System;
using System.Collections;
using Assets.Scripts.Framework.Extensions;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    [Serializable]
    public class AnimationManager<T> where T : Enum
    {
        public AnimationData data;
        public Animator animator;

        public bool[] layerLocked;
        public T[] currentAnimation;

        /// <summary>
        ///  T is animation enum
        /// </summary>
        /// <param name="startingAnimation"></param>
        public void Initialize(T startingAnimation)
        {
            var layers = animator.layerCount;
            
            layerLocked = new bool[layers];
            currentAnimation = new T[layers];

            for (int i = 0; i < layers; i++)
            {
                layerLocked[i] = false;
                currentAnimation[i] = startingAnimation;
            }
        }

        public T GetCurrentAnimation(int layer) => currentAnimation[layer];

        public void SetLocked(bool lockLayer, int layer)
        {
            layerLocked[layer] = lockLayer;
        }

        public void Play(T animationEnum, int layer, bool lockLayer = false, bool overrideLock = false,
            float crossFade = 0.2f)
        {
            if (layerLocked[layer] && !overrideLock) return;

            layerLocked[layer] = lockLayer;

            if (currentAnimation[layer].Equals(animationEnum)) return;

            if (!currentAnimation[layer].Equals(animationEnum))
            {
                // Debug.Log("Current Animation: ".Color("lightblue") + $"{currentAnimation[layer]}".Color("red") + " Next Animation: ".Color("lightblue") + $"{animationEnum}".Color("red"));
            }

            currentAnimation[layer] = animationEnum;
            
            var stateHashName = data.EnumToStateHashName(animationEnum);
            animator.CrossFade(stateHashName, crossFade, layer);
        }

        public void DelayedPlay(MonoBehaviour coroutineStarter, T animationEnum, float delay, int layer,
            bool lockLayer = false, bool overrideLock = false, float crossFade = 0.2f)
        {
            if (delay > 0f)
            {
                coroutineStarter.StartCoroutine(Delay());

                IEnumerator Delay()
                {
                    yield return new WaitForSeconds(delay - crossFade);
                    Play(animationEnum, layer, lockLayer, overrideLock, crossFade);
                }
            }
        }
    }
}