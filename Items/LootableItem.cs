using System;
using System.Collections.Generic;
using Assets.Scripts.ScriptableObjectsScripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Items
{
    // UI For lootable item in loot window
    public class LootableItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public TMP_Text itemName;
        public Image itemIcon;

        public RectTransform backgroundTransformDescription;
        public ItemDescription itemDescription;

        public event Action<LootableItem> ItemOnClick = delegate {  };

        public ItemData itemData;

        [Serializable]
        public struct ItemDescription
        {
            public TMP_Text itemName;
            public TMP_Text itemSlot;
            public TMP_Text itemType;
            public TMP_Text weaponDamage;
            public TMP_Text weaponSpeed;
            public TMP_Text weaponDps;
            public TMP_Text itemStats;

            public TMP_Text description;

            // Add Flavor text ("Forged in the depths of Moria")
            // Add Extra ability text (Chance on hit: Deal an extra attack)
            public void ResetDescription()
            {
                itemName.text = string.Empty;
                itemSlot.text = string.Empty;
                itemType.text = string.Empty;
                weaponDamage.text = string.Empty;
                weaponSpeed.text = string.Empty;
                weaponDps.text = string.Empty;
                itemStats.text = string.Empty;
            }
            
            public void SetBackgroundSize(RectTransform background)
            {
                var value = weaponDamage.ToString().Length;
                var value1 = weaponSpeed.ToString().Length;
                
                var stringFormat = "{0," + value1 + "}";
                var stringFormat1 = "{0," + (value + 2) / 2 + "}";
                
                // TODO: Might just have everything on the same side, to skip format
                var type = string.Format(stringFormat, itemType.text);
                var wSpeed = string.Format(stringFormat1, weaponSpeed.text);
                
                var name = itemName.text;
                var slot = '\n' + itemSlot.text;
                var wDamage = '\n' + weaponDamage.text;
                var wDps = '\n' + weaponDps.text;
                var stats = '\n' + itemStats.text;

                var allTexts = new List<string> { name, slot, type, wDamage, wSpeed, wDps, stats };
                    
                string descriptionText = null;
                foreach (var text in allTexts)
                {
                    if (string.IsNullOrWhiteSpace(text)) continue;
                    descriptionText += text;
                }
                
                description.SetText(descriptionText);

                description.ForceMeshUpdate();
                var vector2 = description.GetRenderedValues();
                background.sizeDelta = vector2;
            }
        }

        // TODO: Anchor tooltip to mouse position
        // TODO: Tooltip class for all tooltip's
        // Aka tooltip class that does:
        // 
        // description.SetText(descriptionText);
        // description.ForceMeshUpdate();
        // var vector2 = description.GetRenderedValues();
        // background.sizeDelta = vector2;
        // (background = rectTransform that lives in the helper class)
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            backgroundTransformDescription.gameObject.SetActive(true);
            itemDescription.SetBackgroundSize(backgroundTransformDescription);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            backgroundTransformDescription.gameObject.SetActive(false);
        }

        // Clicking on the item in loot window
        public void OnItemClick()
        {
            ItemOnClick.Invoke(this);
        }
    }
}