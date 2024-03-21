using System;
using Agricultura.Data;

namespace Agricultura {
    [Serializable]
    public class PlantItem : QuantifiedItem {
        public PlantItem(BaseData plantData, int quantity) : base(plantData, quantity) { }
        public PlantItem(string plantId, int quantity) : base(DataManager.GetData<PlantData>(plantId), quantity) { }
        public override Item Copy(int quantity = 0) {
            return new PlantItem(Data, quantity > 0 ? quantity : Quantity);
        }

        public override SerializedItem ToSerialized() {
            return new SerializedPlantItem() {
                Id = Id,
                Quantity = Quantity
            };
        }
    }
}