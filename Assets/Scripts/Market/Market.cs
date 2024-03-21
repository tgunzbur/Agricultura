using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class Market : MonoBehaviour {
        public List<Stall> stalls;
        public PlayerStall playerStall;

        private void Awake() {
            stalls = new List<Stall>();
            foreach (Transform child in transform) {
                if (child.TryGetComponent(out Stall stall)) {
                    stalls.Add(stall);
                } else if (child.TryGetComponent(out PlayerStall playerStall)) {
                    this.playerStall = playerStall;
                }
            }
        }

        public void FromSerialized(SerializedMarket serializedMarket) {
            for (int count = 0; count < serializedMarket.Stalls.Count; count++) {
                stalls[count].FromSerialized(serializedMarket.Stalls[count]);
            }
            playerStall.FromSerialized(serializedMarket.PlayerStall);
        }

        public SerializedMarket ToSerialized() {
            return new SerializedMarket() {
                Stalls = stalls.Select(stall => stall.ToSerialized()).ToList(),
                PlayerStall = playerStall.ToSerialized()
            };
        }

        public void Reset() {
            stalls.ForEach(stall => stall.Reset());
            playerStall.Reset();
        }
    }
}