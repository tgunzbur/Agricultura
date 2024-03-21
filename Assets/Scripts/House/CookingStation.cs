using System;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public abstract class CookingStation : MonoBehaviour {
        public const int UNLOCK_PRICE = 50; //TODO Change price

        private bool unlocked;
        private CookStationData data;

        private void Start() {
            data = DataManager.GetData<CookStationData>(GetCookingStationId());
        }

        public abstract string GetCookingStationId();

        public void OnClickCookingStation() {
            if (unlocked) {
                CookingStationUIManager.Get().ShowStationWindow(data);
            } else {
                CookingStationUIManager.Get().ShowUnlockWindow(this);
            }
        }

        public void UnlockStation() {
            //TODO Update visually
            InventoryManager.Get().RemoveMoney(UNLOCK_PRICE);
            unlocked = true;
        }

        public void FromSerialized(SerializedCookingStation serializedCookingStation) {
            if (GetCookingStationId() != serializedCookingStation.Id) {
                Debug.LogError($"Initialized from save cooking station doesn't match id save:{serializedCookingStation.Id}, actual:{GetCookingStationId()}");
                return;
            }
            unlocked = serializedCookingStation.Unlocked;
            //TODO Update visually depending on unlocked value
        }

        public SerializedCookingStation ToSerialized() {
            return new SerializedCookingStation() {
                Unlocked = unlocked,
                Id = GetCookingStationId()
            };
        }

        public void Reset() {
            unlocked = false;
            //TODO More thing (reset visually)
        }
    }
}