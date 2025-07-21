using Assets.Scripts.Managers;
using Assets.Scripts.ScriptableObjectsScripts;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class ItemDrop : MonoBehaviour
    {
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        // public InventoryController inventory;

        private ItemData m_ItemData;
        private float m_DistanceToPlayer;

        private void Start()
        {
            // inventory = ServiceLocator.Global.TryGet<InventoryController>();
        }

        private void Update()
        {
            m_DistanceToPlayer =
                Vector3.Distance(transform.position, GameManager.Singleton.playerCharacter.transform.position);

            if (gameObject.activeInHierarchy && m_DistanceToPlayer < 1f)
            {
                DestroyItemObject();
            }
        }

        private void DestroyItemObject()
        {
            gameObject.SetActive(false);
            // inventory.InsertRandomItem(m_ItemData);
        }

        public void SetData(ItemData data)
        {
            m_ItemData = data;

            meshFilter.mesh = data.itemMesh;
            meshRenderer.material = data.itemMaterial;
            meshRenderer.material.mainTexture = data.itemSprite.texture;
        }
    }
}