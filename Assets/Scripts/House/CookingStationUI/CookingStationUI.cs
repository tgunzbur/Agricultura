using System;
using System.Collections.Generic;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class CookingStationUI : MonoBehaviour {
        public virtual void StartGame(List<FoodData> ingredients, int quality) {
            throw new Exception("Can't start game of a unimplemented cooking station");
        }
    }
}