using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class BookUIManager : MonoSingleton<BookUIManager> {
        public enum BookCategoryType {
            FOOD,
            PLANT
        }

        private const int RECIPE_BY_PAGE = 6;

        [SerializeField] private GameObject bookWindow;
        [SerializeField] private Transform recipesParent;
        [SerializeField] private GameObject previousPageButton;
        [SerializeField] private GameObject nextPageButton;
        [SerializeField] private GameObject plantBookMarkParent;
        [SerializeField] private GameObject foodBookMarkParent;
        [SerializeField] private GameObject categoriesParent;

        private RecipeUI[] recipeItems = new RecipeUI[RECIPE_BY_PAGE];

        private Action<RecipeData> currentOnClickRecipe;
        private List<RecipeData> currentRecipes;
        private BookCategoryType currentCategory;
        private string currentSubCategory;
        private bool currentCanChangeSubCategory;
        private int currentPage;
        private int maxPage => Mathf.CeilToInt((float)currentRecipes.Count / RECIPE_BY_PAGE);
        private bool isFirstPage => currentPage <= 1;
        private bool isLastPage => currentPage >= maxPage - 1;

        private void Awake() {
            SetInstance(this);

            int index = 0;
            foreach (Transform child in recipesParent) {
                recipeItems[index++] = child.GetComponent<RecipeUI>();
            }
        }

        public void ShowBook(Action<RecipeData> clickCallBack, BookCategoryType category, bool canChangeCategory = true, string subCategory = null, bool canChangeSubCategory = true) {
            currentOnClickRecipe = clickCallBack;

            bookWindow.SetActive(true);
            categoriesParent.SetActive(canChangeCategory);
            currentCanChangeSubCategory = canChangeSubCategory;

            ChangeCategory(category, true);
            if (!string.IsNullOrWhiteSpace(subCategory)) {
                ChangeSubCategory(subCategory, true);
            }
        }

        public void HideBook() {
            currentOnClickRecipe = null;
            currentCategory = BookCategoryType.PLANT;
            currentSubCategory = null;

            bookWindow.SetActive(false);
        }

        public void OnClickFoodBook() => ChangeCategory(BookCategoryType.FOOD);
        public void OnClickPlantBook() => ChangeCategory(BookCategoryType.PLANT);

        private void ChangeCategory(BookCategoryType category, bool ignorePrevious = false) {
            if (!ignorePrevious && currentCategory == category) {
                return;
            }
            currentCategory = category;
            currentSubCategory = null;
            foodBookMarkParent.SetActive(currentCategory == BookCategoryType.FOOD && currentCanChangeSubCategory);
            plantBookMarkParent.SetActive(currentCategory == BookCategoryType.PLANT && currentCanChangeSubCategory);

            currentRecipes = GetRecipes(currentCategory, currentSubCategory).ToList();
            UpdateBook();
        }

        public void OnClickPanBookMark() => ChangeSubCategory("PAN");
        public void OnClickSaucePanBookMark() => ChangeSubCategory("SAUCE_PAN");
        public void OnClickMixerBookMark() => ChangeSubCategory("MIXER");
        public void OnClickOvenBookMark() => ChangeSubCategory("OVEN");
        public void OnClickCuttingBoardBookMark() => ChangeSubCategory("CUTTING_BOARD");

        private void ChangeSubCategory(string subCategory, bool ignorePrevious = false) {
            if (!ignorePrevious && currentSubCategory == subCategory) {
                return;
            }
            currentSubCategory = subCategory;

            currentRecipes = GetRecipes(currentCategory, currentSubCategory).ToList();
            UpdateBook();
        }

        public void OnClickPreviousPage() {
            if (isFirstPage) {
                throw new Exception("Try to change page when current page is first page");
            }
            currentPage--;
            UpdateRecipes();
        }

        public void OnClickNextPage() {
            if (isLastPage) {
                throw new Exception("Try to change page when current page is last page");
            }
            currentPage++;
            UpdateRecipes();
        }

        private void UpdateBook() {
            currentPage = 0;
            UpdateRecipes();
        }

        private void UpdateRecipes() {
            for (int index = 0; index < RECIPE_BY_PAGE; index++) {
                int recipeIndex = currentPage * RECIPE_BY_PAGE + index;
                UpdateRecipe(recipeItems[index], recipeIndex < currentRecipes.Count ? currentRecipes[recipeIndex] : null);
            }

            previousPageButton.SetActive(!isFirstPage);
            nextPageButton.SetActive(!isLastPage);
        }

        private void UpdateRecipe(RecipeUI recipeUI, RecipeData data) {
            recipeUI.gameObject.SetActive(data != null);
            if (data == null) {
                return;
            }

            recipeUI.UpdateRecipe(data, currentOnClickRecipe);
        }

        private IEnumerable<RecipeData> GetRecipes(BookCategoryType category, string subCategory) {
            return category switch {
                BookCategoryType.FOOD => BookManager.Get().GetRecipes(DataType.FoodData).Where(
                                recipe => string.IsNullOrWhiteSpace(subCategory) ||
                                recipe is FoodRecipeData foodRecipe && foodRecipe.Furniture == subCategory),
                BookCategoryType.PLANT => BookManager.Get().GetRecipes(DataType.PlantData),
                _ => throw new Exception($"Get recipes of a unknown type [{category}]!")
            };
        }
    }
}