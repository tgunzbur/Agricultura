using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agricultura.Data {
    [Serializable]
    public class StallData : BaseData {
        public List<string> SellableItems = new ();

        public StallData() : this("") { }
        public StallData(string id) : base(id) {
            Type = DataType.StallData;
        }

        [NonSerialized] private List<BaseData> sellableItems;
        public List<BaseData> GetSellableItems() {
            if (sellableItems != null) {
                return sellableItems;
            }

            sellableItems = new List<BaseData>();
            foreach (string itemId in SellableItems) {
                BaseData data = DataManager.GetData<BaseData>(itemId);
                if (data != null) {
                    sellableItems.Add(data);
                } else {
                    Debug.LogWarning($"Stall {Id} contains item [{itemId} which doesn't exist in data!");
                }
            }

            return sellableItems;
        }
    }
}