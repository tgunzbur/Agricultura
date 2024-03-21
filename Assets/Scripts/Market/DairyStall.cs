using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;

namespace Agricultura {
    public class DairyStall : FoodStall {

        public override string GetStallId() {
            return "DAIRY_STALL";
        }

        public override SerializedStall ToSerialized() {
            return new SerializedDairyStall() {
                StallCases = stallCases.Select(stallCase => new SerializedStallCase(stallCase)).ToList()
            };
        }

        public override void FromSerialized(SerializedStall serializedStall) {
            if (serializedStall is not SerializedDairyStall serializedDairyStall) {
                throw new Exception("Try to initialize meat stall from non serialized meat stall!");
            }
            stallCases = serializedDairyStall.StallCases.Select(serializedStallCase => new FoodStallCase(serializedStallCase)).ToList();
        }
    }
}