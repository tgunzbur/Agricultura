using System;

namespace Agricultura.Data {
    public enum AchievementType {
        CollectPlant,
        CookFood,
        SellItem,
        BuyItem,
    }

    [Serializable]
    public abstract class AchievementData : BaseData {
        public AchievementType AchievementType;
        public int Quantity;

        public AchievementData() : this("") { }
        public AchievementData(string id) : base(id) {
            Type = DataType.AchievementData;
        }

        public abstract string GetObjectiveBeautify();
    }

    [Serializable]
    public class CollectPlantAchievementData : AchievementData {
        //PlantId empty mean any plant
        public string PlantId;

        public CollectPlantAchievementData() : this("") { }
        public CollectPlantAchievementData(string id) : base(id) {
            AchievementType = AchievementType.CollectPlant;
        }

        private const string plantText = "plants";
        public override string GetObjectiveBeautify() {
            return $"Collect {Quantity} {(string.IsNullOrWhiteSpace(PlantId) ? plantText : DataManager.GetData<PlantData>(PlantId).Name)}";
        }
    }

    [Serializable]
    public class CookFoodAchievementData : AchievementData {
        //FoodId empty mean any food
        public string FoodId;
        //FoodQuality <= 0 mean any quality
        public int FoodQuality;
        public bool CanBeAutoCook;

        public CookFoodAchievementData() : this("") { }
        public CookFoodAchievementData(string id) : base(id) {
            AchievementType = AchievementType.CookFood;
        }

        private const string foodText = "foods";
        public override string GetObjectiveBeautify() {
            string result = $"Cook {Quantity} {(string.IsNullOrWhiteSpace(FoodId) ? foodText : DataManager.GetData<FoodData>(FoodId).Name)}";
            if (FoodQuality > 0) {
                result += $" with a quality of {FoodQuality}";
            }

            return result;
        }
    }

    [Serializable]
    public class SellItemAchievementData : AchievementData {
        //ItemId empty mean any item
        public string ItemId;
        //ItemQuality <= 0 mean no quality or any
        public int ItemQuality;

        public SellItemAchievementData() : this("") { }
        public SellItemAchievementData(string id) : base(id) {
            AchievementType = AchievementType.SellItem;
        }

        private const string itemsText = "items";
        public override string GetObjectiveBeautify() {
            string result = $"Sell {Quantity} {(string.IsNullOrWhiteSpace(ItemId) ? itemsText : DataManager.GetData<BaseData>(ItemId).Name)}";
            if (ItemQuality > 0) {
                result += $" with a quality of {ItemQuality}";
            }

            return result;
        }
    }
}