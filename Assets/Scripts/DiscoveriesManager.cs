using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class DiscoveriesManager : MonoSingleton<DiscoveriesManager> {
        private HashSet<string> discoveries = new ();

        private void Awake() {
            SetInstance(this);
        }

        public bool AddDiscovery(string id) {
            if (!discoveries.Add(id)) {
                Debug.LogWarning($"Fail to add discovery [{id}]!");
                return false;
            }

            return true;
        }

        public SerializedDiscoveries ToSerialized() {
            return new SerializedDiscoveries() {
                Discoveries = discoveries.ToList()
            };
        }

        public void FromSerialized(SerializedDiscoveries serializedDiscovery) {
            discoveries = serializedDiscovery.Discoveries.ToHashSet();
        }

        public void Reset() {
            discoveries = new HashSet<string>();
        }
    }
}