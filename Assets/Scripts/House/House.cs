using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class House : MonoBehaviour {
        private List<CookingStation> cookingStations;

        private void Awake() {
            cookingStations = new List<CookingStation>();
            foreach (Transform child in transform) {
                if (child.TryGetComponent(out CookingStation cookingStation)) {
                    cookingStations.Add(cookingStation);
                }
            }
        }

        public void FromSerialized(SerializedHouse serializedHouse) {
            for (int count = 0; count < serializedHouse.CookingStations.Count; count++) {
                cookingStations[count].FromSerialized(serializedHouse.CookingStations[count]);
            }
        }

        public SerializedHouse ToSerialized() {
            return new SerializedHouse() {
                CookingStations = cookingStations.Select(cookingStation => cookingStation.ToSerialized()).ToList(),
            };
        }

        public void Reset() {
            cookingStations.ForEach(cookingStation => cookingStation.Reset());
        }
    }
}