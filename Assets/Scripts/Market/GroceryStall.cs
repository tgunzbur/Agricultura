using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;

namespace Agricultura {
    public class GroceryStall : FoodStall {
        public override string GetStallId() {
            return "GROCERY_STALL";
        }

        public override SerializedStall ToSerialized() {
            return new SerializedGroceryStall() {
                StallCases = stallCases.Select(stallCase => new SerializedStallCase(stallCase)).ToList()
            };
        }

        public override void FromSerialized(SerializedStall serializedStall) {
            if (serializedStall is not SerializedGroceryStall serializedGroceryStall) {
                throw new Exception("Try to initialize meat stall from non serialized meat stall!");
            }
            stallCases = serializedGroceryStall.StallCases.Select(serializedStallCase => new FoodStallCase(serializedStallCase)).ToList();
        }
    }
}