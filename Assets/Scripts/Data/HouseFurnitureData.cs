using System;

namespace Agricultura.Data {
    [Serializable]
    public class HouseFurnitureData : BaseData {
        public HouseFurnitureDataType SubType;

        public HouseFurnitureData() : this("") { }
        public HouseFurnitureData(string id) : base(id) {
            Type = DataType.HouseFurnitureData;
            SubType = HouseFurnitureDataType.Default;
        }
    }

    [Serializable]
    public class CookStationData : HouseFurnitureData {
        public int NeededClicks = 4;
        public int InventorySize = 4;

        public CookStationData() : this("") { }
        public CookStationData(string id) : base(id) {
            SubType = HouseFurnitureDataType.CookStationData;
        }
    }
}