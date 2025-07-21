using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class TickManager
    {
        public TickManager(float tickTime)
        {
            m_TickTime = tickTime;
        }
        
        private readonly float m_TickTime;
        private float m_DeltaTime;

        public static event System.Action OnTickEvent = delegate { };

        public void TickUpdate()
        {
            m_DeltaTime += Time.deltaTime;

            if (m_DeltaTime >= m_TickTime)
            {
                OnTickEvent.Invoke();
                m_DeltaTime = 0f;
            }
        }
    }
}
