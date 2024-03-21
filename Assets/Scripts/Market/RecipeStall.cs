using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agricultura {
    public class RecipeStall : Stall {
        private int NB_RECIPES = 10;

        private List<RecipeStallCase> stallCases;

        protected override void Start() {
            base.Start();

            sellableItems = new List<BaseData>();
            foreach (PlantData plantData in DataManager.GetAll<PlantData>(DataType.PlantData).Where(data => !BookManager.Get().HasRecipe(data.Id) && data.Recipe?.Count != 0)) {
                sellableItems.Add(plantData);
            }
            foreach (FoodData foodData in DataManager.GetAll<FoodData>(DataType.FoodData).Where(data => !BookManager.Get().HasRecipe(data.Id) && data.RecipeIngredients?.Count != 0)) {
                sellableItems.Add(foodData);
            }
        }

        public override string GetStallId() {
            return "RECIPE_STALL";
        }

        protected override IEnumerable<StallCase> GetStallCases() {
            return stallCases;
        }

        protected override bool RemoveStallCase(StallCase stallCase) {
            return stallCases.Remove(stallCase as RecipeStallCase);
        }

        public override SerializedStall ToSerialized() {
            return new SerializedRecipeStall() {
                StallCases = stallCases.Select(stallCase => new SerializedStallCase(stallCase)).ToList()
            };
        }

        public override void FromSerialized(SerializedStall serializedStall) {
            if (serializedStall is not SerializedRecipeStall serializedRecipeStall) {
                throw new Exception("Try to initialize meat stall from non serialized meat stall!");
            }
            stallCases = serializedRecipeStall.StallCases.Select(serializedStallCase => new RecipeStallCase(serializedStallCase)).ToList();
        }

        public override void Reset() {
            GenerateStallCases();
        }

        protected override void GenerateStallCases() {
            stallCases = new List<RecipeStallCase>();
            for (int count = 0; count < NB_RECIPES; count++) {
                if (sellableItems == null || sellableItems.Count == 0) {
                    break;
                }
                BaseData data = sellableItems[Random.Range(0, sellableItems.Count)];

                PlantData plantData = data as PlantData;
                FoodData foodData = data as FoodData;
                if (plantData == null && foodData == null) {
                    Debug.LogWarning($"Try to add {data.Id} who isn't a food or plant item in recipe stall case");
                    continue;
                }
                if (BookManager.Get().HasRecipe(data.Id) || (plantData != null && plantData.Recipe?.Count == 0) || (foodData != null && foodData?.RecipeIngredients?.Count == 0)) {
                    sellableItems.Remove(data);
                    count--;
                    continue;
                }

                if (stallCases.Find(stallCase => stallCase.Item.Id == data.Id) != null) {
                    count--;
                    continue;;
                }

                stallCases.Add(new RecipeStallCase() {
                    Item = plantData != null ? new PlantRecipeItem(plantData) : new FoodRecipeItem(foodData),
                    Quantity = 1,
                    Price = data.Price, //TODO Change this
                });
            }
        }
    }
}