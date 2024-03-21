using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class InventoryUIManager : MonoSingleton<InventoryUIManager> {
        private InventoryUI currentInventory;
        private DataType currentCategory;
        private string currentSubCategory;
        private Action<Item> currentOnClickItem;

        private void Awake() {
            SetInstance(this);
        }

        private void Start() {
            InventoryManager.Get().OnInventoryUpdate += UpdateInventory;
        }

        public void ShowInventory(InventoryUI inventoryUI, Action<Item> onClickItem, DataType category, bool canChangeCategory = true) {
            currentInventory = inventoryUI;
            currentOnClickItem = onClickItem;

            inventoryUI.SetActiveFoodCategory(canChangeCategory);
            inventoryUI.SetActivePlantCategory(canChangeCategory);

            ChangeCategory(category);
            ChangeSubCategory(null);
            UpdateInventory();
            currentInventory.ShowInventory();
        }

        public void HideInventory() {
            currentOnClickItem = null;
            currentCategory = DataType.BaseData;
            currentSubCategory = null;

            currentInventory.HideInventory();
            currentInventory = null;
        }

        private void UpdateInventory() {
            IEnumerable<Item> items = currentCategory switch {
                DataType.FoodData => InventoryManager.Get().GetAllItems<FoodItem>().Where(foodItem => string.IsNullOrWhiteSpace(currentSubCategory) || (foodItem.Data as FoodData)?.SubType.ToString() == currentSubCategory),
                DataType.PlantData => InventoryManager.Get().GetAllItems<PlantItem>().Where(plantItem => string.IsNullOrWhiteSpace(currentSubCategory) || (plantItem.Data as PlantData)?.Rarity.ToString() == currentSubCategory),
                _ => InventoryManager.Get().GetAllItems()
            };

            currentInventory.UpdateInventory(items);
        }

        public void OnClickPlantCategory() {
            ChangeCategory(DataType.PlantData);
        }

        public void OnClickPlantSubCategory(int rarity) {
            if (currentCategory != DataType.PlantData) {
                Debug.LogError("Try to change sub category of plant type while not in plant category");
                return;
            }
            ChangeSubCategory(rarity.ToString());
        }

        public void OnClickFoodCategory() {
            ChangeCategory(DataType.FoodData);
        }

        public void OnClickFoodSubCategory(string subType) {
            if (currentCategory != DataType.FoodData) {
                Debug.LogError("Try to change sub category of food type while not in food category");
                return;
            }
            ChangeSubCategory(subType);
        }

        private void ChangeCategory(DataType category, bool ignoreCurrent = false) {
            if (currentInventory == null) {
                Debug.LogWarning("Try to change category without a inventory active");
                return;
            }

            currentCategory = ignoreCurrent || currentCategory != category ? category : DataType.BaseData;
            currentInventory.ChangeCategory(currentCategory);
            ChangeSubCategory("", true);
        }

        private void ChangeSubCategory(string subCategory, bool ignoreCurrent = false) {
            if (currentInventory == null) {
                Debug.LogWarning("Try to change sub category without a inventory active");
                return;
            }

            currentSubCategory = ignoreCurrent || currentSubCategory != subCategory ? subCategory : "";
            currentInventory.ChangeSubCategory(currentCategory, currentSubCategory);
            UpdateInventory();
        }

        public void OnClickItem(Item item) {
            if (currentOnClickItem == null) {
                Debug.LogWarning("Try to call on click item without a null callback!");
                return;
            }
            currentOnClickItem.Invoke(item);
        }
    }
}