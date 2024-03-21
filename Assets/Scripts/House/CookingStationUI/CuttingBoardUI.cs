using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Agricultura.Data;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Agricultura {
    public class CuttingBoardUI : CookingStationUI {
        [Header("Cutting Board Game")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform itemsPath;

        [SerializeField] private float itemMaxSpeed = 3;
        [SerializeField] private float itemMinSpeed = 1;
        [SerializeField] private float itemMaxCD = 2;
        [SerializeField] private float itemMinCD = 0.75f;

        [SerializeField] private RectTransform centerPos;
        [SerializeField] private RectTransform spawnPos;
        [SerializeField] private RectTransform endPos;
        [SerializeField] private Animator knife;

        private bool gameRunning;
        private List<RectTransform> currentItems;
        private int totalItems;
        private float score;
        private int cutCount;
        private void Awake() {
            gameRunning = false;
        }

        public override void StartGame(List<FoodData> ingredients, int quality) {
            if (gameRunning) {
                throw new Exception("A game is already taking place");
            }

            FoodData result = DataManager.GetFoodFromFoodRecipe(string.Join("-", ingredients.Select(food => food.Id).OrderBy(id => id)) + "-CUTTING_BOARD");
            StartCoroutine(StartCuttingBoardGame(result, ingredients, quality));
        }

        private void FixedUpdate() {
            if (!gameRunning) {
                return;
            }

            if (currentItems == null) {
                return;
            }
            for (int count = 0; count < currentItems.Count; count++) {
                if (currentItems[count].position.x > endPos.position.x) {
                    cutCount++;
                    CookingStationUIManager.Get().IncrementProgression(100f / totalItems);
                    Destroy(currentItems[count].gameObject);
                    currentItems.RemoveAt(count--);
                }
            }
        }

        private IEnumerator StartCuttingBoardGame(FoodData result, List<FoodData> ingredients, int quality) {
            score = 0;
            cutCount = 0;
            totalItems = ingredients.Count;
            currentItems = new List<RectTransform>();
            DateTime nextItemSpawn = DateTime.Now;
            gameRunning = true;
            while (currentItems.Count > 0 || ingredients.Count > 0) {
                if (ingredients.Count > 0 && nextItemSpawn < DateTime.Now) {
                    int randomIndex = Random.Range(0, ingredients.Count);
                    RectTransform item = Instantiate(itemPrefab, itemsPath).GetComponent<RectTransform>();
                    item.GetComponent<CuttingBoardItem>().SetSpeed((endPos.position - spawnPos.position).normalized * Random.Range(itemMinSpeed, itemMaxSpeed));
                    item.GetComponent<Image>().sprite = DataManager.GetIcon(ingredients[randomIndex].Id);
                    item.name = ingredients[randomIndex].Id;
                    item.position = spawnPos.position;
                    nextItemSpawn = DateTime.Now + TimeSpan.FromSeconds(Random.Range(itemMinCD, itemMaxCD));

                    ingredients.RemoveAt(randomIndex);
                    currentItems.Add(item);
                }

                yield return new WaitForFixedUpdate();
            }

            gameRunning = false;
            int finalQuality = Mathf.RoundToInt(score / cutCount * quality);
            FoodItem finalResult;
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

        public void OnClickCutButton() {
            if (!gameRunning) {
                return;
            }
            knife.SetTrigger("Cut");
            RectTransform itemInCenter = null;
            foreach (RectTransform item in currentItems) {
                Rect itemRect = new (item.TransformPoint(item.rect.center), item.rect.size);
                Rect centerRect = new (centerPos.TransformPoint(centerPos.rect.center), centerPos.rect.size);
                if (centerRect.Overlaps(itemRect)) {
                    itemInCenter = item;
                    break;
                }
            }
            cutCount++;
            if (itemInCenter != null) {
                score += 1 - (centerPos.position - itemInCenter.transform.position).magnitude / (itemInCenter.sizeDelta.x / 2 + centerPos.sizeDelta.x / 2);
                //TODO Show text depending on score (Perfect, Good, OK, Bad, ...)
                CookingStationUIManager.Get().IncrementProgression(100f / totalItems);
                currentItems.Remove(itemInCenter);
                Destroy(itemInCenter.gameObject);
                return;
            }

            //TODO Show text "Miss"
        }
    }
}