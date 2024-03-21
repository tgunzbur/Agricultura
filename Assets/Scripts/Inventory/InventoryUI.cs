using System;
using System.Collections.Generic;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class InventoryUI : MonoBehaviour {
        [Header("Inventory Parameters")]
        [SerializeField] private GameObject window;

        [SerializeField] private Transform itemsParent;
        [SerializeField] private GameObject itemPrefab;

        [SerializeField] private GameObject plantCategory;
        [SerializeField] private GameObject foodCategory;
        [SerializeField] private GameObject plantSubCategories;
        [SerializeField] private GameObject foodSubCategories;

        [SerializeField] private GameObject selectedPlantCategory;
        [SerializeField] private GameObject selectedFoodCategory;

        [SerializeField] private GameObject plantSubCategory1;
        [SerializeField] private GameObject plantSubCategory2;
        [SerializeField] private GameObject plantSubCategory3;
        [SerializeField] private GameObject plantSubCategory4;
        [SerializeField] private GameObject plantSubCategory5;

        [SerializeField] private GameObject foodSubCategoryVegetable;
        [SerializeField] private GameObject foodSubCategoryMeat;
        [SerializeField] private GameObject foodSubCategoryIngredient;
        [SerializeField] private GameObject foodSubCategoryDish;

        private DynamicUIListPoolable<Item> itemsList;
        private void Awake() {
            if (itemsList != null) {
                return;
            }
            itemsList = new DynamicUIListPoolable<Item>(itemsParent, itemPrefab, UpdateItem);
            window.SetActive(false);
        }

        public void ShowInventory() {
            window.SetActive(true);
        }

        public void HideInventory() {
            window.SetActive(false);
        }

        public void UpdateInventory(IEnumerable<Item> items) {
            itemsList ??= new DynamicUIListPoolable<Item>(itemsParent, itemPrefab, UpdateItem);
            itemsList.ReplaceAllItems(items);
        }

        private void UpdateItem(GameObject item, Item data) {
            item.GetComponent<ItemUI>().UpdateItem(data, InventoryUIManager.Get().OnClickItem);
        }

        public void SetActivePlantCategory(bool state) {
            plantCategory.SetActive(state);
        }

        public void SetActiveFoodCategory(bool state) {
            foodCategory.SetActive(state);
        }

        public void ChangeCategory(DataType category) {
            plantSubCategories.SetActive(category == DataType.PlantData);
            foodSubCategories.SetActive(category == DataType.FoodData);
            selectedPlantCategory.SetActive(category == DataType.PlantData);
            selectedFoodCategory.SetActive(category == DataType.FoodData);
        }

        public void ChangeSubCategory(DataType category, string subCategory) {
            switch (category) {
                case DataType.FoodData:
                    foodSubCategoryVegetable.SetActive(subCategory == FoodType.Vegetable.ToString());
                    foodSubCategoryMeat.SetActive(subCategory == FoodType.Meat.ToString());
                    foodSubCategoryIngredient.SetActive(subCategory == FoodType.Ingredient.ToString());
                    foodSubCategoryDish.SetActive(subCategory == FoodType.Dish.ToString());
                    break;
                case DataType.PlantData:
                    plantSubCategory1.SetActive(subCategory == "1");
                    plantSubCategory2.SetActive(subCategory == "2");
                    plantSubCategory3.SetActive(subCategory == "3");
                    plantSubCategory4.SetActive(subCategory == "4");
                    plantSubCategory5.SetActive(subCategory == "5");
                    break;
                case DataType.BaseData:
                    break;
                default:
                    Debug.LogWarning($"Category of inventory [{category}] is not managed!");
                    break;
            }
        }
    }
}