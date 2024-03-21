using System;
using Agricultura.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class ItemUI : MonoBehaviour {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text quantity;
        [SerializeField] private GameObject[] qualities;
        [SerializeField] private Button button;

        public void UpdateItem(Item item, Action<Item> onClick) {
            UpdateItem(item.Data);
            if (qualities.Length > 0 && item is FoodItem foodItem) {
                for (int count = 0; count < FoodItem.MAX_QUALITY; count++) {
                    qualities[count].SetActive(count + 1 == foodItem.Quality);
                }
            }

            if (quantity != null && item is QuantifiedItem quantifiedItem) {
                quantity.text = quantifiedItem.Quantity.ToString();
                quantity.gameObject.SetActive(true);
            }

            button?.onClick.AddListener(() => onClick?.Invoke(item));
        }

        public void UpdateItem(BaseData data) {
            name = data.Id;

            if (icon != null) {
                icon.sprite = data.GetIcon();
            }

            if (background != null) {
                if (data is PlantData plantData) {
                    background.sprite = DataManager.Get().GetBackgroundItemRarity(plantData.Rarity);
                } else {
                    background.sprite = DataManager.Get().GetBackgroundItemRarity(0);
                }
            }

            if (qualities.Length > 0) {
               for (int count = 0; count < FoodItem.MAX_QUALITY; count++) {
                   qualities[count].SetActive(false);
               }
            }
            quantity?.gameObject.SetActive(false);
            button?.onClick.RemoveAllListeners();
        }
    }
}