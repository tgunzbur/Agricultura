using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Agricultura.Data {
    [Serializable]
    public class PlantData : BaseData {
        public List<string> Recipe = new ();
        public string ResultFoodId = "";

        public int GrowthTime = 15;
        public int Rarity = 1;

        public PlantData() : this("") { }
        public PlantData(string id) : base(id) {
            Type = DataType.PlantData;
        }

        [NonSerialized] private string hashedRecipe;
        public string GetRecipe() {
            if (hashedRecipe == null) {
                if (Recipe.Count <= 0) {
                    hashedRecipe = "";
                    return hashedRecipe;
                }
                Recipe.Sort(string.CompareOrdinal);
                hashedRecipe = string.Join("-", Recipe);
            }

            return hashedRecipe;
        }

        [NonSerialized] private FoodData foodResult;
        public FoodData GetFoodResult() {
            if (foodResult == null) {
                foodResult = DataManager.GetData<FoodData>(ResultFoodId);
            }

            return foodResult;
        }
    }
}