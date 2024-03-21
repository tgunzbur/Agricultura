using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agricultura {
    public class FoodStallCase : StallCase {
        public FoodStallCase() { }
        public FoodStallCase(SerializedStallCase stallCase) {
            Item = stallCase.Item.ToUnserialized();
            Quantity = stallCase.Quantity;
            Price = stallCase.Price;
        }
    }

    public class RecipeStallCase : StallCase {
        public RecipeStallCase() { }
        public RecipeStallCase(SerializedStallCase stallCase) {
            Item = stallCase.Item.ToUnserialized();
            Quantity = stallCase.Quantity;
            Price = stallCase.Price;
        }
    }

    public abstract class StallCase {
        public Item Item;
        public int Quantity;
        public int Price;
    }

    public abstract class Stall : MonoBehaviour {
        protected List<BaseData> sellableItems;

        //TODO Make the generation of quantity, price and item sold (and the update)
        protected virtual void Start() {
            sellableItems = DataManager.GetData<StallData>(GetStallId()).GetSellableItems();
            TimeManager.Get().OnDayChange += GenerateStallCases;
        }

        public abstract string GetStallId();

        protected abstract IEnumerable<StallCase> GetStallCases();
        protected abstract bool RemoveStallCase(StallCase stallCase);
        public abstract SerializedStall ToSerialized();
        public abstract void FromSerialized(SerializedStall serializedStall);
        public abstract void Reset();
        protected abstract void GenerateStallCases();

        public IEnumerable<BaseData> GetItems() {
            return GetStallCases().Select(stallCase => stallCase.Item.Data);
        }

        public int GetStallCasesCount() {
            return GetStallCases().Count();
        }

        public StallCase GetStallCase(int index) {
            StallCase[] stallCases = GetStallCases().ToArray();
            if (index < 0 || index >= stallCases.Length) {
                Debug.LogWarning("Try to get a stallCase out of range");
                return null;
            }
            return stallCases[index];
        }

        public bool BuyItem(string id, int quantity) {
            if (quantity <= 0) {
                Debug.LogError($"Try to buy item [{id}] at [{GetStallId()}] with a zero or negative quantity!");
            }

            StallCase stallCase = GetStallCases().FirstOrDefault(stallCase => stallCase.Item.Data.Id == id);
            if (stallCase == null || stallCase.Quantity < quantity) {
                Debug.LogError($"Try to buy item [{id}] at [{GetStallId()}] stall, but this stall doesn't have this item");
                return false;
            }
            if (InventoryManager.Get().GetMoney() < stallCase.Price * quantity) {
                Debug.LogError($"Try to buy item [{id}] for [{stallCase.Price * quantity}]$ at [{GetStallId()}] stall, but don't have enough money");
                return false;
            }

            InventoryManager.Get().AddItem(stallCase.Item.Copy(quantity));

            stallCase.Quantity -= quantity;
            if (stallCase.Quantity == 0) {
                if (!RemoveStallCase(stallCase)) {
                    Debug.LogError($"Fail to remove stall case with item [{stallCase.Item.Data.Id}] from [{GetStallId()}]!");
                }
            }

            InventoryManager.Get().RemoveMoney(stallCase.Price * quantity);
            return true;
        }

        public void OnClickStall() {
            StallUIManager.Get().ShowStall(this);
        }
    }

    public abstract class FoodStall : Stall {
        protected List<FoodStallCase> stallCases = new();

        protected override IEnumerable<StallCase> GetStallCases() {
            return stallCases;
        }

        protected override bool RemoveStallCase(StallCase stallCase) {
            return stallCases.Remove(stallCase as FoodStallCase);
        }

        public override void Reset() {
            GenerateStallCases();
        }

        protected override void GenerateStallCases() {
            stallCases = new List<FoodStallCase>();
            foreach (BaseData data in sellableItems) {
                if (data is not FoodData foodData) {
                    Debug.LogWarning($"Try to add {data.Id} who isn't a food item in food stall case");
                    continue;
                }

                int randomQuantity = Random.Range(25, 100); //TODO Change this
                stallCases.Add(new FoodStallCase() {
                    Item = new FoodItem(foodData, randomQuantity, Random.Range(1, FoodItem.MAX_QUALITY + 1)), //TODO Change this
                    Quantity = randomQuantity,
                    Price = data.Price, //TODO Change this
                });
            }
        }
    }
}