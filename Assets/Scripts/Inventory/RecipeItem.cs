using System;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public abstract class RecipeItem : Item {
        public RecipeItem(BaseData baseData) : base(baseData) {
            if (baseData is not (FoodData or PlantData)) {
                Debug.LogError($"Create a recipe item for data [{baseData.Id}] which isn't food or plant!");
            }
        }
    }

    public class PlantRecipeItem : RecipeItem {
        public PlantRecipeItem(PlantData plantData) : base(plantData) { }
        public PlantRecipeItem(string plantId) : base(DataManager.GetData<PlantData>(plantId)) { }

        public override Item Copy(int quantity = 0) {
            return new PlantRecipeItem(Data as PlantData);
        }

        public override SerializedItem ToSerialized() {
            return new SerializedPlantRecipeItem() {
                Id = Id
            };
        }
    }

    public class FoodRecipeItem : RecipeItem {
        public FoodRecipeItem(FoodData foodData) : base(foodData) { }
        public FoodRecipeItem(string foodId) : base(DataManager.GetData<FoodData>(foodId)) { }

        public override Item Copy(int quantity = 0) {
            return new FoodRecipeItem(Data as FoodData);
        }

        public override SerializedItem ToSerialized() {
            return new SerializedFoodRecipeItem() {
                Id = Id
            };
        }
    }
}