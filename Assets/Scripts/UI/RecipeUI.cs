using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class RecipeUI : MonoBehaviour {
        [SerializeField] private Image recipeIcon;
        [SerializeField] private TMP_Text recipeName;
        [SerializeField] private Image furnitureIcon;
        [SerializeField] private Button button;
        [SerializeField] private Transform ingredientsParent;
        [SerializeField] private ItemUI ingredientPrefab;

        private DynamicUIListPoolable<string> ingredientsList;

        private void Awake() {
            if (ingredientsParent != null && ingredientPrefab != null) {
                ingredientsList = new DynamicUIListPoolable<string>(ingredientsParent, ingredientPrefab.gameObject, UpdateIngredient);
            }
        }

        private void UpdateIngredient(GameObject item, string id) {
            ItemUI itemUI = item.GetComponent<ItemUI>();
            itemUI.UpdateItem(DataManager.GetData<BaseData>(id));
        }

        public void UpdateRecipe(RecipeData data, Action<RecipeData> onClick) {
            FoodRecipeData foodData = data as FoodRecipeData;
            BaseData resultData;
            if (foodData != null) {
                resultData = DataManager.GetData<FoodData>(data.Result);
            } else {
                resultData = DataManager.GetData<PlantData>(data.Result);
            }
            UpdateRecipe(resultData, data.RecipeHash.Split('-').SkipLast(foodData != null ? 1 : 0), foodData?.Furniture);

            button?.onClick.RemoveAllListeners();
            button?.onClick.AddListener(() => onClick?.Invoke(data));
        }

        public void UpdateRecipe(List<string> ingredients, string furniture) {
            FoodData resultData = DataManager.GetFoodFromFoodRecipe(ingredients == null ? null : string.Join("-", ingredients.OrderBy(id => id)) + "-" + furniture);
            UpdateRecipe(resultData, ingredients, furniture);
        }

        private void UpdateRecipe(BaseData resultData, IEnumerable<string> ingredients, string furniture) {
            if (recipeIcon != null) {
                recipeIcon.sprite = resultData.GetIcon();
            }

            if (recipeName != null) {
                recipeName.text = resultData.Name;
            }

            ingredientsList.ReplaceAllItems(ingredients);

            if (furnitureIcon != null) {
                furnitureIcon.gameObject.SetActive(!string.IsNullOrWhiteSpace(furniture));
                if (!string.IsNullOrWhiteSpace(furniture)) {
                    furnitureIcon.sprite = DataManager.GetIcon(furniture);
                }
            }
        }
    }
}