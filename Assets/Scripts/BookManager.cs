using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class RecipeData {
        public DataType Type;
        public string Result;
        public string RecipeHash;

        public virtual SerializedRecipe ToSerialized() {
            return new SerializedRecipe() {
                Type = Type.ToString(),
                Result = Result,
                RecipeHash = RecipeHash,
            };
        }
    }

    public class FoodRecipeData : RecipeData {
        public string Furniture;
        public int CookedCount;

        public override SerializedRecipe ToSerialized() {
            return new SerializedFoodRecipe() {
                Type = Type.ToString(),
                Result = Result,
                RecipeHash = RecipeHash,
                CookedCount = CookedCount,
                Furniture = Furniture
            };
        }
    }

    public class BookManager : MonoSingleton<BookManager> {
        private List<RecipeData> recipes = new ();

        private void Awake() {
            SetInstance(this);
        }

        public void AddRecipe(FoodData foodData) {
            recipes.Add(new FoodRecipeData() {
                Type = DataType.FoodData,
                Result = foodData.Id,
                RecipeHash = foodData.GetRecipe(),
                Furniture = foodData.RecipeFurniture,
                CookedCount = 0
            });
        }

        public void AddRecipe(PlantData plantData) {
            recipes.Add(new RecipeData() {
                Type = DataType.PlantData,
                Result = plantData.Id,
                RecipeHash = plantData.GetRecipe()
            });
        }

        public void CookRecipe(string id) {
            RecipeData recipe = recipes.Find(recipe => recipe.Result == id);
            if (recipe is FoodRecipeData foodRecipe) {
                foodRecipe.CookedCount++;
            } else {
                Debug.LogWarning($"Try to cook a non-food recipe [{id}]");
            }
        }

        public int GetCookCount(string id) {
            RecipeData recipe = recipes.Find(recipe => recipe.Result == id);
            if (recipe is FoodRecipeData foodRecipe) {
                return foodRecipe.CookedCount;
            } else {
                Debug.LogError($"Try to get cooked cook of a non-food recipe [{id}]");
                return 0;
            }
        }

        public bool HasRecipe(string id) {
            return recipes.Find(recipe => recipe.Result == id) != null;
        }

        public IEnumerable<RecipeData> GetRecipes(DataType dataType) {
            return recipes.Where(recipe => recipe.Type == dataType);
        }

        public SerializedBook ToSerialized() {
            return new SerializedBook() {
                Recipes = recipes.Select(recipe => recipe.ToSerialized()).ToList()
            };
        }

        public void FromSerialized(SerializedBook serializedBook) {
            recipes = serializedBook.Recipes.Select(recipe => recipe.ToUnserialized()).ToList();
        }

        public void Reset() {
            recipes = new List<RecipeData>();
        }
    }
}