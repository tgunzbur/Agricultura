using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class PlayerStallCase : StallCase {
        public float SecondSinceAdded;

        public PlayerStallCase() { }
        public PlayerStallCase(SerializedPlayerStallCase playerStallCase) {
            Item = playerStallCase.Item.ToUnserialized();
            Quantity = playerStallCase.Quantity;
            Price = playerStallCase.Price;
            SecondSinceAdded = playerStallCase.SecondSinceAdded;
        }
    }

    public class PlayerStall : MonoBehaviour {
        public const int MAX_STALL_CASE = 16;

        private List<PlayerStallCase> stallCases = new();

        public void OnClickStall() {
            PlayerStallUIManager.Get().ShowStallWindow(this);
        }

        public int GetStallCasePrice() {
            return 100 * GetStallCasesCount(); //TODO Change this
        }

        public int BuyStallCase() {
            if (GetStallCasesCount() >= MAX_STALL_CASE) {
                Debug.LogError($"Try to buy new stall case at player stall, but already have max stall case");
                return -1;
            }
            int price = GetStallCasePrice();
            if (InventoryManager.Get().GetMoney() < price) {
                Debug.LogError($"Try to buy new stall case for [{price}]$ at player stall, but don't have enough money");
                return -1;
            }
            stallCases.Add(null);
            InventoryManager.Get().RemoveMoney(price);
            return stallCases.Count - 1;
        }

        public BaseData AddItemToSell(int stallCaseIndex, Item item, int quantity, int price) {
            PlayerStallCase stallCase = GetStallCase(stallCaseIndex);
            if (stallCase != null) {
                Debug.LogError("Try to add item to a non-empty stall case in player stall");
                return null;
            }

            stallCases[stallCaseIndex] = new PlayerStallCase() {
                Item = item.Copy(),
                Quantity = quantity,
                Price = price,
                SecondSinceAdded = 0
            };
            return stallCases[stallCaseIndex].Item.Data;
        }

        public bool RemoveItemFromSell(StallCase stallCase) {
            int index = stallCases.FindIndex(x => x == stallCase);
            stallCases[index] = null;
            InventoryManager.Get().AddItem(stallCase.Item, stallCase.Quantity);
            return true;
        }

        public PlayerStallCase GetStallCase(int index) {
            if (index < 0 || index >= stallCases.Count) {
                Debug.LogError($"Try to get stall case at index [{index}] in player stall but index is out of range!");
                return null;
            }
            return stallCases[index];
        }

        public int GetStallCasesCount() {
            return stallCases.Count;
        }

        public SerializedPlayerStall ToSerialized() {
            return new SerializedPlayerStall() {
                StallCases = stallCases.Select(stallCase => stallCase == null ? null : new SerializedPlayerStallCase(stallCase)).ToArray()
            };
        }

        public void FromSerialized(SerializedPlayerStall serializedStall) {
            stallCases = serializedStall.StallCases.Select(serializedStallCase => serializedStallCase == null ? null : new PlayerStallCase(serializedStallCase)).ToList();
        }

        public void Reset() {
            stallCases = new List<PlayerStallCase> {
                null
            };
        }
    }
}