using System;
using Agricultura.Data;

namespace Agricultura {
    public class FoodItem : QuantifiedItem {
        public const int MAX_QUALITY = 3;

        public readonly int Quality;

        public FoodItem(BaseData foodData, int quantity, int quality) : base(foodData, quantity) {
            Quality = quality;
        }
        public FoodItem(string foodId, int quantity, int quality) : this(DataManager.GetData<FoodData>(foodId), quantity, quality) { }

        public override Item Copy(int quantity = 0) {
            return new FoodItem(Data, quantity > 0 ? quantity : Quantity, Quality);
        }

        public override SerializedItem ToSerialized() {
            return new SerializedFoodItem() {
                Id = Id,
                Quantity = Quantity,
                Quality = Quality
            };
        }
    }
}