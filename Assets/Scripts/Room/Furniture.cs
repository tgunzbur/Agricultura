using System;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public abstract class Furniture : MonoBehaviour {
        private static int furnitureCount;
        public readonly int UniqueId = furnitureCount++;

        public string Id => data.Id;
        protected BaseData data;
        public void SetData(BaseData newData) {
            if (data != null) {
                throw new Exception("Can't change Furniture data when it was already set");
            }

            data = newData;
        }
        public static bool operator ==(Furniture a, Furniture b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(Furniture a, Furniture b) => !(a == b);

        public bool Equals(Furniture other) {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return UniqueId == other.UniqueId;
        }

        public override bool Equals(object obj) => Equals(obj as Furniture);

        public override int GetHashCode() {
            return UniqueId;
        }
    }

    public abstract class InteractableFurniture : Furniture {
        public abstract void OnClickWithItem(Item item);
        public abstract void OnClickWithEmptyHand();
    }
}