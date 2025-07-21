using System;
using System.Collections;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class StaminaBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        public float GetStamina => slider.value;
        public event Action StopSprintEvent = delegate {  };
        private bool m_Sprinting;
        private float m_RegenRate;
        private float m_ReduceRate;
        private float m_WaitUntilStartRegen;

        public void SetBaseSettings(PlayerStats.StaminaStruct stamina)
        {
            SetMaxStamina(stamina.baseStamina);
            m_ReduceRate = stamina.reduceRate;
            m_RegenRate = stamina.regenRate;
            m_WaitUntilStartRegen = stamina.waitUntilStartRegen;
        }
        
        // Set max stamina
        private void SetMaxStamina(float sta)
        {
            slider.maxValue = sta;
        }

        private IEnumerator Sprinting()
        {
            if (slider.value <= 0f) m_Sprinting = false;
            
            while (slider.value > 0f && m_Sprinting)
            {
                if (slider.value < 1f)
                {
                    StopSprintEvent.Invoke();
                    m_Sprinting = false;
                }
                
                slider.value -= m_ReduceRate * Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(m_WaitUntilStartRegen);

            while (slider.value < slider.maxValue)
            {
                if (m_Sprinting) yield break;

                slider.value += m_RegenRate * Time.deltaTime;
                yield return null;
            }
        }

        public void SprintStart()
        {
            m_Sprinting = true;
            StartCoroutine(Sprinting());
        }

        public void SprintStop()
        {
            m_Sprinting = false;
        }
    }
}