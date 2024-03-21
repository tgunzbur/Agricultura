using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Agricultura {
    public class MixerUI : CookingStationUI {
        [SerializeField] private float timeInAreaNeeded;
        [SerializeField] private float areaMinSpeed;
        [SerializeField] private float areaMaxSpeed;
        [SerializeField] private float areaChangeDirectionMinCD;
        [SerializeField] private float areaChangeDirectionMaxCD;

        [SerializeField] private float clickStrength;
        [SerializeField] private float cursorFallStrength;

        [SerializeField] private RectTransform mixerArea;
        [SerializeField] private GameObject itemPrefab;

        [SerializeField] private RectTransform cursor;
        [SerializeField] private RectTransform area;

        private bool gameRunning;
        private Vector2 areaStartPos;
        private Vector2 cursorStartPos;
        private float currentSpeed;
        private DateTime nextChangeDirection;
        private float timeOutOfArea;
        private float timeInArea;
        private void Awake() {
            gameRunning = false;
            cursorStartPos = cursor.anchoredPosition;
            areaStartPos = area.anchoredPosition;
        }

        private void Update() {
            if (!gameRunning) {
                return;
            }

            if ((cursor.anchoredPosition - area.anchoredPosition).magnitude <= area.sizeDelta.y / 2) {
                timeInArea += Time.deltaTime;
                CookingStationUIManager.Get().SetProgression(timeInArea / timeInAreaNeeded * 100);
            } else {
                timeOutOfArea += Time.deltaTime;
            }

            if (timeInArea > timeInAreaNeeded) {
                EndGame();
            }

            if (nextChangeDirection < DateTime.Now) {
                currentSpeed *= -1;
                nextChangeDirection = DateTime.Now.AddSeconds(Random.Range(areaChangeDirectionMinCD, areaChangeDirectionMaxCD));
            }
            MoveArea(area.anchoredPosition + Vector2.up * (currentSpeed * Time.deltaTime));
            MoveCursor(cursor.anchoredPosition - Vector2.up * cursorFallStrength);
        }

        private void MoveCursor(Vector2 newPos) {
            Vector2 parentSize = cursor.parent.GetComponent<RectTransform>().sizeDelta;
            newPos.y = Mathf.Clamp(newPos.y, -parentSize.y / 2, parentSize.y / 2);
            cursor.anchoredPosition = newPos;
        }

        private void MoveArea(Vector2 newPos) {
            Vector2 parentSize = area.parent.GetComponent<RectTransform>().sizeDelta;
            newPos.y = Mathf.Clamp(newPos.y, -parentSize.y / 2, parentSize.y / 2);
            area.anchoredPosition = newPos;
        }

        private List<FoodData> currentIngredients;
        private int currentQuality;
        public override void StartGame(List<FoodData> ingredients, int quality) {
            if (gameRunning) {
                throw new Exception("A game is already taking place");
            }

            currentIngredients = ingredients;
            currentQuality = quality;
            cursor.anchoredPosition = cursorStartPos;
            area.anchoredPosition = areaStartPos;
            currentSpeed = Random.Range(areaMinSpeed, areaMaxSpeed) * Mathf.Sign(Random.value - 0.5f);
            nextChangeDirection = DateTime.Now.AddSeconds(Random.Range(areaChangeDirectionMinCD, areaChangeDirectionMaxCD));
            timeInArea = 0;
            timeOutOfArea = 0;

            foreach (FoodData ingredient in currentIngredients) {
                RectTransform ingredientRect = Instantiate(itemPrefab, mixerArea).GetComponent<RectTransform>();
                ingredientRect.GetComponent<Image>().sprite = DataManager.GetIcon(ingredient.Id);
                ingredientRect.anchoredPosition = mixerArea.anchoredPosition;
            }

            gameRunning = true;
        }

        private void EndGame() {
            gameRunning = false;
            foreach (Transform child in mixerArea) {
                Destroy(child.gameObject);
            }

            int finalQuality = Mathf.RoundToInt((1 - timeOutOfArea / (timeInAreaNeeded + timeOutOfArea)) * currentQuality);

            FoodItem finalResult;
            FoodData result = DataManager.GetFoodFromFoodRecipe(string.Join("-", currentIngredients.Select(food => food.Id).OrderBy(id => id)) + "-MIXER");
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

        public void OnClickMix() {
            MoveCursor(cursor.anchoredPosition + Vector2.up * clickStrength);
        }

        public void OnItemHitBlade(Rigidbody2D item) {
            Vector2 sizeParent = cursor.parent.GetComponent<RectTransform>().sizeDelta;
            Vector2 force = new (
                Random.Range(-250f, 250f),
                Random.Range(1000f, 2500f) * ((sizeParent.y / 2 + cursor.anchoredPosition.y) / sizeParent.y * 2f)
                );
            item.velocity = force;
        }
    }
}