using Agricultura.Data;

namespace Agricultura {
    public abstract class Achievement {
        public AchievementData Data;
        public int Progression;

        public Achievement(AchievementData data, int progression = 0) {
            Data = data;
            Progression = progression;
        }

        public SerializedAchievement ToSerialized() {
            return new SerializedAchievement() {
                AchievementId = Data.Id,
                Progression = Progression
            };
        }
    }

    public class CollectPlantAchievement : Achievement {
        public CollectPlantAchievement(AchievementData data, int progression = 0) : base(data, progression) { }

        public void OnCollectPlant(PlantItem collectedItem) {

        }
    }

    public class CookFoodAchievement : Achievement {
        public CookFoodAchievement(AchievementData data, int progression = 0) : base(data, progression) { }

        public void OnCookFood(FoodItem cookedItem, bool autoCook) {

        }
    }

    public class SellItemAchievement : Achievement {
        public SellItemAchievement(AchievementData data, int progression = 0) : base(data, progression) { }
        
        public void OnSellItem(Item soldItem, int price) {

        }
    }
}