using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Agricultura {
    public class OvenUI : CookingStationUI {
        [SerializeField] private float minTime = 3;
        [SerializeField] private float maxTime = 8;

        [SerializeField] private Transform ovenArea;
        [SerializeField] private GameObject itemPrefab;

        [SerializeField] private Transform modeWheel;
        [SerializeField] private Image targetModeImage;
        [SerializeField] private int modeCount;
        [SerializeField] private List<Sprite> modesList;

        [SerializeField] private Transform temperatureWheel;
        [SerializeField] private Image targetTemperatureImage;
        [SerializeField] private int temperatureCount;
        [SerializeField] private List<Sprite> temperaturesList;

        [SerializeField] private Transform timeWheel;
        [SerializeField] private Image targetTimeImage;
        [SerializeField] private int timeCount;
        [SerializeField] private List<Sprite> timesList;

        private float timeToSetButtons;
        private bool gameRunning;
        private int currentMode;
        private int currentTemperature;
        private int currentTime;

        private int targetMode;
        private int targetTemperature;
        private int targetTime;

        private List<FoodData> currentIngredients;
        private int currentQuality;
        private void Update() {
            if (!gameRunning) {
                return;
            }

            float progression = (currentMode == targetMode ? 0 : 100 / 3f) +
                                (currentTemperature == targetTemperature ? 0 : 100 / 3f) +
                                (currentTime == targetTime ? 0 : 100 / 3f);
            CookingStationUIManager.Get().SetProgression(progression);
            if (progression >= 1) {
                EndGame();
            }
            timeToSetButtons += Time.deltaTime;
        }

        private void EndGame() {
            gameRunning = false;
            foreach (Transform child in ovenArea) {
                Destroy(child.gameObject);
            }

            int finalQuality = Mathf.RoundToInt((1 - (timeToSetButtons - minTime / maxTime)) * currentQuality);

            FoodItem finalResult;
            FoodData result = DataManager.GetFoodFromFoodRecipe(string.Join("-", currentIngredients.Select(food => food.Id).OrderBy(id => id)) + "-OVEN");
            if (finalQuality > 0 && result.Id != "UNKNOWN_MIXTURE") {
                finalResult = new FoodItem(result, 1, finalQuality);
                BookManager.Get().CookRecipe(result.Id);
            } else {
                //TODO Show to the player "You have miss the recipe" or "This recipe led to nothing"
                finalResult = new FoodItem("UNKNOWN_MIXTURE", 1, 0);
            }
            InventoryManager.Get().AddItem(finalResult);
            CookingStationUIManager.Get().ShowCookingResult(finalResult);
            CookingStationUIManager.Get().HideGamesWindow();
        }

        public override void StartGame(List<FoodData> ingredients, int quality) {
            if (gameRunning) {
                throw new Exception("A game is already taking place");
            }
            currentIngredients = ingredients;
            currentQuality = quality;

            timeToSetButtons = 0;

            currentMode = Random.Range(0, modeCount);
            while (targetMode == currentMode) {
                targetMode = Random.Range(0, modeCount);
            }
            targetModeImage.sprite = modesList[targetMode];
            SetWheel(modeWheel, modeCount, currentMode);
            currentTemperature = Random.Range(0, temperatureCount);
            targetTemperature = currentTemperature;
            while (targetTemperature == currentTemperature) {
                targetTemperature = Random.Range(0, temperatureCount);
            }
            targetTemperatureImage.sprite = temperaturesList[targetTime];
            SetWheel(temperatureWheel, temperatureCount, currentTemperature);
            currentTime = Random.Range(0, timeCount);
            targetTime = currentTime;
            while (targetTime == currentTime) {
                targetTime = Random.Range(0, timeCount);
            }
            targetTimeImage.sprite = timesList[targetTime];
            SetWheel(timeWheel, timeCount, currentTime);

            foreach (FoodData ingredient in ingredients) {
                RectTransform ingredientRect = Instantiate(itemPrefab, ovenArea).GetComponent<RectTransform>();
                ingredientRect.GetComponent<Image>().sprite = DataManager.GetIcon(ingredient.Id);
                //TODO Position it randomly inside
            }

            gameRunning = true;
        }

        private void SetWheel(Transform wheel, int count, int index) {
            float rotation = (2 * Mathf.PI) / count * index;
            wheel.rotation = quaternion.Euler(0, 0, rotation);
        }

        public void OnClickModeWheel() {
            currentMode = (currentMode + 1) % modeCount;
            SetWheel(modeWheel, modeCount, currentMode);
        }

        public void OnClickTemperatureWheel() {
            currentTemperature = (currentTemperature + 1) % temperatureCount;
            SetWheel(temperatureWheel, temperatureCount, currentTemperature);
        }

        public void OnClickTimeWheel() {
            currentTime = (currentTime + 1) % timeCount;
            SetWheel(timeWheel, timeCount, currentTime);
        }
    }
}