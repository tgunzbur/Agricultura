using System;
using System.Collections.Generic;
using Agricultura.Data;
using UnityEngine;

namespace Agricultura {
    public class GameManager : MonoSingleton<GameManager> {
        [Header("Rooms")]
        [SerializeField] private Field field;
        [SerializeField] private House house;
        [SerializeField] private Market market;

        public Action<PlantItem> OnCollectPlant; //TODO Call at good time
        public Action<FoodItem, bool> OnCookFood;
        public Action<Item, int> OnSellItem;

        private void Awake() {
            SetInstance(this);
        }

        private void Start() {
            SerializedGame serializedGame = DataManager.LoadSave();
            if (serializedGame != null && !serializedGame.TutorialUnFinish) {
                FromSerialized(serializedGame);
                UIManager.Get().Reset();
            } else {
                Reset();
            }
        }

        public void Reset() {
            field.Regenerate();
            house.Reset();
            market.Reset();
            InventoryManager.Get().Reset();
            BookManager.Get().Reset();
            DiscoveriesManager.Get().Reset();
            AchievementsManager.Get().Reset();
            TimeManager.Get().Reset();

            UIManager.Get().Reset();
        }

        public void OnApplicationPause(bool pauseStatus) {
            if (pauseStatus) {
                Save();
            }
        }

        public void OnApplicationFocus(bool hasFocus) {
            if (!hasFocus) {
                Save();
            }
        }

        public void OnApplicationQuit() {
            if (CookingStationUIManager.Get().HasIngredients()) {
                CookingStationUIManager.Get().ClearIngredients();
            }

            Save();
        }

        public void Save() {
            DataManager.Save(ToSerialized());
        }

        private SerializedGame ToSerialized() {
            return new SerializedGame() {
                Field = field.ToSerialized(),
                House = house.ToSerialized(),
                Market = market.ToSerialized(),
                Inventory = InventoryManager.Get().ToSerialized(),
                Book = BookManager.Get().ToSerialized(),
                Discoveries = DiscoveriesManager.Get().ToSerialized(),
                Achievements = AchievementsManager.Get().ToSerialized(),
                Time = TimeManager.Get().GetTime(),
                TutorialUnFinish = false,
            };
        }

        private void FromSerialized(SerializedGame serializedGame) {
            field.FromSerialized(serializedGame.Field);
            house.FromSerialized(serializedGame.House);
            market.FromSerialized(serializedGame.Market);
            InventoryManager.Get().FromSerialized(serializedGame.Inventory);
            BookManager.Get().FromSerialized(serializedGame.Book);
            DiscoveriesManager.Get().FromSerialized(serializedGame.Discoveries);
            AchievementsManager.Get().FromSerialized(serializedGame.Achievements);
            TimeManager.Get().FromSerialized(serializedGame.Time);
        }
    }
}