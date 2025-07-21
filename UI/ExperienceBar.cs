using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ExperienceBar : MonoBehaviour
    {
        public Slider slider;
        public TMP_Text levelText;

        private float m_PlayerExperience;
        private float m_ExpToNextLevel = 20f;
        private float m_ExpForLastLevel;
        private float m_ExpLeft;
        private int m_Level = 0;

        private void Awake()
        {
            SetMaxExp(m_ExpToNextLevel);
        }
        
        // Set level text
        private void SetLevelText(string text)
        {
            levelText.SetText(text);
        }

        // Set max Exp
        private void SetMaxExp(float exp)
        {
            slider.maxValue = exp;
            slider.value = 0f;
        }

        public void AddExp(float amount)
        {
            m_ExpLeft = 0f;
            m_PlayerExperience += amount;
            slider.value += amount;

            if (m_PlayerExperience >= m_ExpToNextLevel)
            {
                m_ExpLeft = m_PlayerExperience - m_ExpToNextLevel;
                LevelUp();
            }
        }

        private void UpdateExpToNextLevel()
        {
            m_ExpForLastLevel = m_ExpToNextLevel;
            m_ExpToNextLevel = m_ExpForLastLevel + (m_ExpToNextLevel * 0.5f);
            m_PlayerExperience = 0f;

            m_Level++;
            SetMaxExp(m_ExpToNextLevel);
            SetLevelText($"LvL. {m_Level}");

            AddExp(m_ExpLeft);
        }

        private void LevelUp()
        {
            // Debug.Log("Setting game state to Level up".Color("green"));
            // GameManager.Instance.UpdateGameState(Enums.GameState.LevelUp);
            UpdateExpToNextLevel();
        }
    }
}