using System.Collections;
using Assets.Scripts.Framework.EventBus;
using Assets.Scripts.Framework.Structs;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class ExpDrop : MonoBehaviour
    {
        public int expAmount;
        private const float k_PickupRange = 2f;

        private Vector3 m_PlayerLocation;
        private float m_DistanceToPlayer;
        private bool m_Active;
        private bool m_ReachedPlayer;

        public void UpdateExperienceAmount(int amount) => expAmount = amount;

        public void OnDisable() => m_Active = true;

        public void Update()
        {
            m_PlayerLocation = GameManager.Singleton.playerCharacter.transform.position;
            m_DistanceToPlayer = Vector3.Distance(transform.position, m_PlayerLocation);

            if (m_Active && m_DistanceToPlayer < k_PickupRange)
            {
                StartCoroutine(FlyToPlayer());
            }
        }

        private IEnumerator FlyToPlayer()
        {
            while (!m_ReachedPlayer)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_PlayerLocation, 10f * Time.deltaTime);

                if (m_DistanceToPlayer < 0.1f) DestroyExpObj();

                yield return null;
            }
        }

        private void DestroyExpObj()
        {
            m_Active = false;
            gameObject.SetActive(false);
            EventBus<ExperienceEvent>.Raise(new ExperienceEvent {experience = expAmount});
            // UIManager.Singleton.AddExperience(expAmount);
        }
    }
}