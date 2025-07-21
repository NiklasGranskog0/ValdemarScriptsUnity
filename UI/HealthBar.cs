using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;

        public bool isRegenerating;
        private readonly float m_RegenRate = 0.1f;

        public IEnumerator RegenHealth()
        {
            isRegenerating = true;
            
            while (isRegenerating)
            {
                if (slider.value >= slider.maxValue) isRegenerating = false;
                
                slider.value += m_RegenRate * Time.deltaTime;
                yield return null;
            }
        }

        // Set health
        public void SetMaxHealth(float health)
        {
            if (!slider) return;
            slider.maxValue = health;
            slider.value = health;
        }

        // Update health
        public void SetCurrentHealth(float health)
        {
            slider.value = health;
        }
    }
}
