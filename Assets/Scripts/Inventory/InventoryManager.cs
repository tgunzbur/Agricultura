using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class InventoryManager : MonoSingleton<InventoryManager> {
        private enum SelectedItemType {
            HAND,
            BAG,
            SCYTHE
        }

        private List<Item> items = new ();
        private int money;
        private int gems;

        private SelectedItemType currentSelectedType;
        private Item currentBagItem;
        private ScytheItem scythe;

        public Action<int> OnMoneyChange;
        public Action<int> OnGemsChange;
        public Action<Item> OnAddItem;
        public Action OnInventoryUpdate;

        private void Awake() {
            SetInstance(this);
        }

        public int GetMoney() {
            return money;
        }

        public void AddMoney(int quantity) {
            if (quantity <= 0) {
                Debug.LogWarning($"Try to add [{quantity}] to money but it's zero or a negative value!");
                return;
            }
            money += quantity;
            OnMoneyChange?.Invoke(money);
        }

        public void RemoveMoney(int quantity) {
            if (quantity <= 0) {
                Debug.LogWarning($"Try to remove [{quantity}] from money but it's zero or a negative value!");
                return;
            }
            money -= quantity;
            OnMoneyChange?.Invoke(money);
        }

        public int GetGems() {
            return gems;
        }

        public void AddGems(int quantity) {
            if (quantity <= 0) {
                Debug.LogWarning($"Try to add [{quantity}] to gems but it's zero or a negative value!");
                return;
            }
            gems += quantity;
            OnGemsChange?.Invoke(gems);
        }

        public void RemoveGems(int quantity) {
            if (quantity <= 0) {
                Debug.LogWarning($"Try to remove [{quantity}] from gems but it's zero or a negative value!");
                return;
            }
            gems -= quantity;
            OnGemsChange?.Invoke(gems);
        }

        public void AddItem(Item item, int quantity = 0) {
            if (item is RecipeItem recipeItem) {
                if (!BookManager.Get().HasRecipe(recipeItem.Id)) {
                    FoodData foodData = recipeItem.Data as FoodData;
                    PlantData plantData = recipeItem.Data as PlantData;
                    if (!string.IsNullOrEmpty(foodData?.GetRecipe())) {
                        BookManager.Get().AddRecipe(foodData);
                    } else if (!string.IsNullOrEmpty(plantData?.GetRecipe())) {
                        BookManager.Get().AddRecipe(plantData);
                    }
                } else {
                    Debug.LogWarning($"Try to add a recipe in inventory for {item.Data.Id} which is already known!");
                }
                return;
            }
            if (item is FoodItem foodItem) {
                quantity = quantity > 0 ? quantity : foodItem.Quantity;
                FoodItem matchItem = items.Find(x => x.Id == item.Id && x is FoodItem foodX && foodX.Quality == foodItem.Quality) as FoodItem;
                if (matchItem != null) {
                    matchItem.Quantity += quantity;
                } else {
                    if (!BookManager.Get().HasRecipe(foodItem.Id)) {
                        FoodData foodData = foodItem.Data as FoodData;
                        if (!string.IsNullOrEmpty(foodData.GetRecipe())) {
                            BookManager.Get().AddRecipe(foodData);
                        }
                    }
                    Item newItem = foodItem.Copy(quantity);
                    items.Add(newItem);
                    OnAddItem?.Invoke(newItem);
                }
            } else if (item is PlantItem plantItem) {
                quantity = quantity > 0 ? quantity : plantItem.Quantity;
                PlantItem matchItem = items.Find(x => x.Id == item.Id && x is PlantItem) as PlantItem;
                if (matchItem != null) {
                    matchItem.Quantity += quantity;
                } else {
                    if (!BookManager.Get().HasRecipe(plantItem.Id)) {
                        PlantData plantData = plantItem.Data as PlantData;
                        if (!string.IsNullOrEmpty(plantData.GetRecipe())) {
                            BookManager.Get().AddRecipe(plantData);
                        }
                    }
                    Item newItem = plantItem.Copy(quantity);
                    items.Add(newItem);
                    OnAddItem?.Invoke(newItem);
                }
            } else {
                items.Add(item);
                OnAddItem?.Invoke(item);
            }

            OnInventoryUpdate?.Invoke();
        }

        public void RemoveItem(Item item, int quantity = 0) {
            if (item is FoodItem foodItem) {
                FoodItem match = items.Find(x => x.Id == item.Id && x is FoodItem foodX && foodX.Quality == foodItem.Quality) as FoodItem;
                if (match != null) {
                    match.Quantity -= quantity > 0 ? quantity : foodItem.Quantity;
                    if (match.Quantity <= 0) {
                        items.Remove(match);
                    }
                }
            } else if (item is PlantItem plantItem) {
                PlantItem match = items.Find(x => x.Id == item.Id && x is PlantItem) as PlantItem;
                if (match != null) {
                    match.Quantity -= quantity > 0 ? quantity : plantItem.Quantity;
                    if (match.Quantity <= 0) {
                        items.Remove(match);
                    }
                }
            } else {
                items.Remove(item);
            }
            OnInventoryUpdate?.Invoke();
        }

        public T GetItem<T>(string id) where T : Item {
            return items.Find(x => x.Data.Id == id) as T;
        }

        public IEnumerable<T> GetAllItems<T>() where T : Item {
            return items.OfType<T>();
        }

        public IEnumerable<Item> GetAllItems() {
            return items;
        }

        public Item GetCurrentItem() {
            return currentSelectedType switch {
                SelectedItemType.BAG => currentBagItem,
                SelectedItemType.HAND => null,
                SelectedItemType.SCYTHE => scythe ??= new ScytheItem(),
                _ => null
            };
        }

        public void SetBagItem(Item item) {
            currentSelectedType = SelectedItemType.BAG;
            currentBagItem = item;
        }

        public void SelectBagItem() {
            currentSelectedType = SelectedItemType.BAG;
        }

        public void SelectHand() {
            currentSelectedType = SelectedItemType.HAND;
        }

        public void SelectScythe() {
            currentSelectedType = SelectedItemType.SCYTHE;
        }

        private void EmptyHand() {
            currentSelectedType = SelectedItemType.HAND;
            currentBagItem = null;

            UIManager.Get().OnEmptyHand();
        }

        public void FromSerialized(SerializedInventory serializedInventory) {
            money = serializedInventory.Money;
            items = serializedInventory.Items.Select(item => item.ToUnserialized()).ToList();
        }

        public SerializedInventory ToSerialized() {
            return new SerializedInventory() {
                Money = money,
                Items = items.Select(item => item.ToSerialized()).ToList()
            };
        }

        public void Reset() {
            items.Clear();
        }
    }
}