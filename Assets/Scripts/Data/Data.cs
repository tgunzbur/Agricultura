using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Agricultura.Data {
    public enum DataType {
        BaseData,
        PlantData,
        FoodData,
        HouseFurnitureData,
        StallData,
        AchievementData
    }

    public enum HouseFurnitureDataType {
        Default,
        CookStationData
    }

    [Serializable]
    public class BaseData {
        public DataType Type;
        public string Id;
        public string Name;
        public string Description = "";
        public string IconPath = "";
        public int Price = 2;

        public BaseData() : this("") { }
        public BaseData(string id) {
            Id = id;
            Name = Id.ToLower().FirstCharacterToUpper();
            Type = DataType.BaseData;
        }

        [NonSerialized] private Sprite sprite;
        public Sprite GetIcon() {
            if (sprite == null) {
                sprite = DataManager.GetIcon(Id);
            }
            return sprite;
        }

        public static bool operator ==(BaseData a, BaseData b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }
            if (ReferenceEquals(a, null)) {
                return false;
            }
            if (ReferenceEquals(b, null)) {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(BaseData a, BaseData b) {
            return !(a == b);
        }

        public override bool Equals(object obj) {
            if (obj is not BaseData data) {
                return false;
            }
            return Id == data.Id;
        }

        protected bool Equals(BaseData other) {
            return Id == other.Id;
        }

        public override int GetHashCode() {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public override string ToString() {
            return Name;
        }
    }
}