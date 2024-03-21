using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agricultura.Data {
    [Serializable]
    public class SerializedPlant {
        public Position Position;
        public string Id;
        public float Growth;

        public SerializedPlant() { }
        public SerializedPlant(Position position, Plant plant) {
            Position = position;
            Id = plant.Id;
            Growth = plant.Growth;
        }
    }

    [Serializable]
    public class SerializedField {
        public List<SerializedPlant> Plants = new ();
    }

    [Serializable]
    public class SerializedCookingStation {
        public string Id;
        public bool Unlocked;
    }

    [Serializable]
    public class SerializedHouse {
        public List<SerializedCookingStation> CookingStations;
    }

    [Serializable]
    public class SerializedStallCase {
        public SerializedItem Item;
        public int Quantity;
        public int Price;

        public SerializedStallCase() { }
        public SerializedStallCase(StallCase stallCase) {
            Item = stallCase.Item.ToSerialized();
            Quantity = stallCase.Quantity;
            Price = stallCase.Price;
        }
    }

    [Serializable]
    public abstract class SerializedStall {
        public List<SerializedStallCase> StallCases;
    }

    [Serializable]
    public class SerializedMeatStall : SerializedStall { }

    [Serializable]
    public class SerializedDairyStall : SerializedStall { }

    [Serializable]
    public class SerializedGroceryStall : SerializedStall { }

    [SerializeField]
    public class SerializedRecipeStall : SerializedStall { }

    [Serializable]
    public class SerializedPlayerStallCase {
        public SerializedItem Item;
        public int Quantity;
        public int Price;
        public float SecondSinceAdded;

        public SerializedPlayerStallCase() { }
        public SerializedPlayerStallCase(PlayerStallCase stallCase) {
            Item = stallCase.Item.ToSerialized();
            Quantity = stallCase.Quantity;
            Price = stallCase.Price;
            SecondSinceAdded = stallCase.SecondSinceAdded;
        }
    }

    [Serializable]
    public class SerializedPlayerStall {
        public SerializedPlayerStallCase[] StallCases;
    }

    [Serializable]
    public class SerializedMarket {
        public List<SerializedStall> Stalls;
        public SerializedPlayerStall PlayerStall;
    }

    [Serializable]
    public abstract class SerializedItem {
        public string Id;

        protected SerializedItem() { }
        protected SerializedItem(string id) {
            Id = id;
        }

        public abstract Item ToUnserialized();
    }

    [Serializable]
    public abstract class SerializedQuantifiedItem : SerializedItem {
        public int Quantity;

        protected SerializedQuantifiedItem() { }
        protected SerializedQuantifiedItem(string id, int quantity) : base(id) {
            Quantity = quantity;
        }
    }

    [Serializable]
    public class SerializedFoodItem : SerializedQuantifiedItem {
        public int Quality;

        public SerializedFoodItem() { }
        public SerializedFoodItem(FoodItem item) : base(item.Id, item.Quantity) {
            Quality = item.Quality;
        }

        public override Item ToUnserialized() {
            return new FoodItem(Id, Quantity, Quality);
        }
    }

    [Serializable]
    public class SerializedPlantItem : SerializedQuantifiedItem {
        public SerializedPlantItem() { }
        public SerializedPlantItem(PlantItem item) : base(item.Id, item.Quantity) { }

        public override Item ToUnserialized() {
            return new PlantItem(Id, Quantity);
        }
    }

    [Serializable]
    public abstract class SerializedRecipeItem : SerializedItem {
        public SerializedRecipeItem() { }
        public SerializedRecipeItem(RecipeItem item) : base(item.Id) { }
    }

    [Serializable]
    public class SerializedPlantRecipeItem : SerializedRecipeItem {
        public override Item ToUnserialized() {
            return new PlantRecipeItem(Id);
        }
    }

    [Serializable]
    public class SerializedFoodRecipeItem : SerializedRecipeItem {
        public override Item ToUnserialized() {
            return new FoodRecipeItem(Id);
        }
    }

    [Serializable]
    public class SerializedInventory {
        public int Money;
        public List<SerializedItem> Items;
    }

    public class SerializedRecipe {
        public string Type;
        public string Result;
        public string RecipeHash;

        public virtual RecipeData ToUnserialized() {
            if (!DataType.TryParse(Type, out DataType type)) {
                throw new Exception($"Fail to parse type [{type}] of serialized recipe!");
            }
            return new RecipeData() {
                Type = type,
                Result = Result,
                RecipeHash = RecipeHash
            };
        }
    }

    public class SerializedFoodRecipe : SerializedRecipe {
        public string Furniture;
        public int CookedCount;

        public override RecipeData ToUnserialized() {
            if (!DataType.TryParse(Type, out DataType type)) {
                throw new Exception($"Fail to parse type [{type}] of serialized recipe!");
            }
            return new FoodRecipeData() {
                Type = type,
                Result = Result,
                RecipeHash = RecipeHash,
                CookedCount = CookedCount,
                Furniture = Furniture
            };
        }
    }

    [Serializable]
    public class SerializedBook {
        public List<SerializedRecipe> Recipes;
    }

    [Serializable]
    public class SerializedDiscoveries {
        public List<string> Discoveries;
    }

    [Serializable]
    public class SerializedAchievement {
        public string AchievementId;
        public int Progression;
    }

    [Serializable]
    public class SerializedAchievements {
        public List<SerializedAchievement> Achievements;
    }

    [Serializable]
    public class SerializedGame {
        public SerializedField Field;
        public SerializedHouse House;
        public SerializedMarket Market;
        public SerializedInventory Inventory;
        public SerializedBook Book;
        public SerializedDiscoveries Discoveries;
        public SerializedAchievements Achievements;
        public float Time;
        public bool TutorialUnFinish;
    }
}