using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class StallCaseUI : MonoBehaviour {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemPrice;
        [SerializeField] private Image[] images;
        [SerializeField] private Button button;

        public StallCase StallCase { get; private set; }

        public void ShowStallCase(StallCase stallCase, Action<StallCaseUI> onClick) {
            StallCase = stallCase;
            itemName.text = stallCase?.Item.Data.Name ?? "";
            itemPrice.text = stallCase?.Price.ToString() ?? "";
            SetImages(stallCase?.Item.Data.GetIcon(), stallCase?.Quantity ?? 0);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(this));
        }

        private void SetImages(Sprite sprite, int itemNb) {
            for (int count = 0; count < images.Length; count++) {
                images[count].enabled = sprite != null && count < itemNb;
                images[count].sprite = sprite;
            }
        }
    }
}