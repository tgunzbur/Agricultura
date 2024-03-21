using System;
using System.Collections.Generic;
using System.Linq;

namespace Agricultura.Data {
    public enum FoodType {
        Vegetable,
        Meat,
        Ingredient,
        Dish
    }

    [Serializable]
    public class FoodData : BaseData {
        public List<string> RecipeIngredients = new ();
        public string RecipeFurniture = "";
        public FoodType SubType = FoodType.Ingredient;
        public int NeededCookForAutoCook = 5;

        public FoodData() : this("") { }
        public FoodData(string id) : base(id) {
            Type = DataType.FoodData;
        }

        [NonSerialized] private string hashedRecipe;
        public string GetRecipe() {
            if (hashedRecipe == null) {
                if (RecipeIngredients.Count == 0 || string.IsNullOrWhiteSpace(RecipeFurniture)) {
                    hashedRecipe = "";
                    return hashedRecipe;
                }
                hashedRecipe = string.Join("-", RecipeIngredients.OrderBy(id => id)) + "-" + RecipeFurniture;
            }

            return hashedRecipe;
        }

    }
}