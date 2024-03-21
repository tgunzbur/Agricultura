using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;

namespace Agricultura {
    public class MeatStall : FoodStall {
        public override string GetStallId() {
            return "MEAT_STALL";
        }

        public override SerializedStall ToSerialized() {
            return new SerializedMeatStall() {
                StallCases = stallCases.Select(stallCase => new SerializedStallCase(stallCase)).ToList()
            };
        }

        public override void FromSerialized(SerializedStall serializedStall) {
            if (serializedStall is not SerializedMeatStall serializedMeatStall) {
                throw new Exception("Try to initialize meat stall from non serialized meat stall!");
            }
            stallCases = serializedMeatStall.StallCases.Select(serializedStallCase => new FoodStallCase(serializedStallCase)).ToList();
        }
    }
}