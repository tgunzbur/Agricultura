using System;
using Agricultura.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class SeedCreator : MonoBehaviour {
        private const float TIME_TO_COMPLETE = 10;

        [NonSerialized] private Transform clickBar;

        public PlantData PlantResult;

        public float Load {
            get;
            private set;
        }
        private void Awake() {
            PlantResult = null;
            clickBar = transform.Find("ClickBar");
            if (clickBar == null) {
                throw new Exception("Seed Creator need a child named ClickBar with a slider component");
            }
            UpdateClickUI();
        }

        private void FixedUpdate() {
            if (PlantResult == null || !TimeManager.Get().TimePass) {
                return;
            }

            Load += Time.fixedDeltaTime / TIME_TO_COMPLETE;
            UpdateClickUI();

            if (Load >= 1) {
                OnFullLoad();
                Load = 0;
            }
        }

        private void UpdateClickUI() {
            if (Load <= 0 || Load >= 1) {
                clickBar.gameObject.SetActive(false);
            } else {
                clickBar.gameObject.SetActive(true);
                Slider clickBarSlider = clickBar.Find("Slider").GetComponent<Slider>();
                clickBarSlider.value = Load;
            }
        }

        private void OnFullLoad() {
            PlantItem result = new (PlantResult, 2);
            PlantResult = null;

            DropUIManager.Get().OnDropItem(transform, result);
        }

        public void OnClickSeedCreator(Item item) {
            if (PlantResult != null) {
                ErrorUIManager.Get().ShowFullError(transform);
                return;
            }

            PlantResult = DataManager.GetPlantFromFood(item.Id);
            Load = 0;
            if (PlantResult == null) {
                ErrorUIManager.Get().ShowForbiddenError(transform);
            } else {
                InventoryManager.Get().RemoveItem(item, 1);
            }
        }
    }
}