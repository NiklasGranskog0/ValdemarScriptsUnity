using Assets.Scripts.Framework.Tags;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class EnterArea : MonoBehaviour
    {
        public TitleAreaUI titleAreaUI;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.k_Player))
            {
                titleAreaUI.OnPlayerEnter();
            }
        }

    }
}
