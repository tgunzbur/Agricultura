using System;
using Agricultura.Data;

namespace Agricultura {
    public abstract class Item {
        private static int itemCount = 0;
        private readonly int UniqueId;

        public readonly string Id;
        public readonly BaseData Data;

        protected Item() {
            UniqueId = itemCount++;
        }
        protected Item(BaseData data) {
            UniqueId = itemCount++;

            Id = data.Id;
            Data = data;
        }
        protected Item(Item item) : this(item.Data) { }

        public abstract Item Copy(int quantity = 0);

        public static bool operator ==(Item a, Item b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(Item a, Item b) => !(a == b);

        public bool Equals(Item other) {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return UniqueId == other.UniqueId;
        }

        public override bool Equals(object obj) => Equals(obj as Item);

        public override int GetHashCode() {
            return UniqueId;
        }
        
        public abstract SerializedItem ToSerialized();
    }

    public abstract class QuantifiedItem : Item {
        public int Quantity;

        protected QuantifiedItem(BaseData data, int quantity) : base(data) {
            Quantity = quantity;
        }
        protected QuantifiedItem(QuantifiedItem item, int quantity) : base(item) {
            Quantity = quantity;
        }
    }
}