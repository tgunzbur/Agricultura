using System;
using Agricultura.Data;

namespace Agricultura {
    public class ScytheItem : Item {
        public ScytheItem() : base(DataManager.GetData<BaseData>("SCYTHE")) { }
        public override Item Copy(int quantity = 0) {
            return new ScytheItem();
        }

        public override SerializedItem ToSerialized() {
            throw new Exception("Can't serialized Scythe item!");
        }
    }
}