using System.Collections;
using Assets.Scripts.Framework.Extensions;
using UnityEngine;

namespace Assets.Scripts.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData Data { get; private set; }
        private AudioSource m_AudioSource;
        private Coroutine m_PlayingCoroutine;

        private void Awake()
        {
            m_AudioSource = gameObject.GetOrAdd<AudioSource>();
        }

        public void Initialize(SoundData data)
        {
            Data = data;
            m_AudioSource.clip = data.audioClip;
            m_AudioSource.outputAudioMixerGroup = data.audioMixerGroup;
            m_AudioSource.loop = data.loop;
            m_AudioSource.playOnAwake = data.playOnAwake;

            m_AudioSource.mute = data.mute;
            m_AudioSource.bypassEffects = data.bypassEffects;
            m_AudioSource.bypassListenerEffects = data.bypassListenerEffects;
            m_AudioSource.bypassReverbZones = data.bypassReverbZones;

            m_AudioSource.priority = data.priority;
            m_AudioSource.volume = data.volume;
            m_AudioSource.pitch = data.pitch;
            m_AudioSource.panStereo = data.panStereo;
            m_AudioSource.spatialBlend = data.spatialBlend;
            m_AudioSource.reverbZoneMix = data.reverbZoneMix;
            m_AudioSource.dopplerLevel = data.dopplerLevel;
            m_AudioSource.spread = data.spread;

            m_AudioSource.minDistance = data.minDistance;
            m_AudioSource.maxDistance = data.maxDistance;

            m_AudioSource.ignoreListenerVolume = data.ignoreListenerVolume;
            m_AudioSource.ignoreListenerPause = data.ignoreListenerPause;
            m_AudioSource.rolloffMode = data.rolloffMode;
        }

        public void Play()
        {
            if (m_PlayingCoroutine != null)
            {
                StopCoroutine(m_PlayingCoroutine);
            }
            
            m_AudioSource.Play();
            m_PlayingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        public void Stop()
        {
            if (m_PlayingCoroutine != null)
            {
                StopCoroutine(m_PlayingCoroutine);
                m_PlayingCoroutine = null;
            }
            
            m_AudioSource.Stop();
            gameObject.SetActive(false);
        }

        private IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => m_AudioSource.isPlaying);
            gameObject.SetActive(false);
        }
    }
}
