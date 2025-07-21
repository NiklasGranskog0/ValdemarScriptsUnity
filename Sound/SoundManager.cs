using System.Collections.Generic;
using Assets.Scripts.Framework;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    public class SoundManager : SingletonInstance<SoundManager>
    {
        private ObjectPool m_SoundEmitterPool;
        public readonly Queue<SoundEmitter> frequentSoundEmitters = new();

        [SerializeField] private GameObject soundEmitterPrefab;
        [SerializeField] private uint initialQuantity = 10;
        [SerializeField] private int maxSoundInstances = 30;

        private void Awake()
        {
            m_SoundEmitterPool = new ObjectPool(initialQuantity, soundEmitterPrefab, gameObject.transform);
        }

        public void PlaySound(SoundData data)
        {
            if (!CanPlaySound(data)) return;

            var emitter = RentSoundEmitter(true);
            emitter.Initialize(data);

            emitter.transform.position = data.audioPosition != null ? data.audioPosition.position : Vector3.zero;

            if (data.frequentSound) frequentSoundEmitters.Enqueue(emitter);
            emitter.Play();
        }

        private bool CanPlaySound(SoundData data)
        {
            if (!data.frequentSound) return true;

            if (frequentSoundEmitters.Count >= maxSoundInstances &&
                frequentSoundEmitters.TryDequeue(out var emitter))
            {
                try
                {
                    emitter.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }

                return false;
            }

            return true;
        }

        private SoundEmitter RentSoundEmitter(bool activate)
        {
            var soundEmitter = m_SoundEmitterPool.Rent(activate);
            return soundEmitter.TryGetComponent<SoundEmitter>(out var emitter) ? emitter : null;
        }
    }
}
