using System;
using Assets.Scripts.Framework.ServiceManagement;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class GameTimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [Header("In Minutes")] [SerializeField] private float startTime;

        public string GetTimeAsString => TimeSpan.FromSeconds(GetCurrentTimeFloat).ToString(@"mm\:ss");
        public float GetCurrentSeconds => TimeSpan.FromSeconds(GetCurrentTimeFloat).Seconds;
        public float GetCurrentMinutes => TimeSpan.FromSeconds(GetCurrentTimeFloat).Minutes;
        public float GetCurrentTimeFloat { get; private set; }

        public float GetCurrentEndlessTime { get; private set; }
        public float GetCurrentEndlessTimeSeconds => TimeSpan.FromSeconds(GetCurrentEndlessTime).Seconds;
        
        public bool StartTimer { get; set; }
        public bool TimedMode { get; set; }
        public bool EndlessMode { get; set; }

        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global);
            GetCurrentTimeFloat = startTime * 60f;

            Debug.Log("Game timer not doing anything because game mode isn't set");
            TimedMode = false;
            EndlessMode = false;
        }

        private void Update()
        {
            if (!StartTimer) return;
            
            if (timerText && TimedMode)
            {
                GetCurrentTimeFloat -= Time.deltaTime;
                var timeSpan = TimeSpan.FromSeconds(GetCurrentTimeFloat);
                timerText.SetText(timeSpan.ToString(@"mm\:ss"));
            }

            if (timerText && EndlessMode)
            {
                GetCurrentEndlessTime += Time.deltaTime;
                var timeSpan = TimeSpan.FromSeconds(GetCurrentEndlessTime);
                timerText.SetText(timeSpan.ToString(@"mm\:ss"));
            }
        }
    }
}
