using System;
using System.Collections.Generic;
using System.Linq;
using Agricultura;
using Agricultura.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CookingStationUIManager : MonoSingleton<CookingStationUIManager> {
    [Header("Cooking Station Window")]
    [SerializeField] private GameObject cookingStationWindow;
    [SerializeField] private GameObject confirmationCookWindow;
    [SerializeField] private GameObject cuttingBoardImage;
    [SerializeField] private GameObject mixerImage;
    [SerializeField] private GameObject ovenImage;
    [SerializeField] private GameObject saucePanImage;
    [SerializeField] private GameObject panImage;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private Button cookButton;
    [SerializeField] private Button autoCookButton;
    [SerializeField] private GameObject autoCookInfo;
    [SerializeField] private TMP_Text autoCookText;
    [SerializeField] private RecipeUI cookingStationRecipe;

    [Header("Unlock Window")]
    [SerializeField] private GameObject unlockWindow;
    [SerializeField] private TMP_Text unlockText;
    [SerializeField] private TMP_Text unlockPrice;
    [SerializeField] private Button unlockConfirmButton;

    [Header("Inventory Parameters")]
    [SerializeField] private InventoryUI inventory;

    [Header("MiniGame Window")]
    [SerializeField] private GameObject stationsGameWindowParent;
    [SerializeField] private TMP_Text gameProgressionText;
    [SerializeField] private Slider gameProgressionSlider;
    [SerializeField] private RecipeUI gameRecipe;

    [SerializeField] private CuttingBoardUI cuttingBoardWindow;
    [SerializeField] private MixerUI mixerWindow;
    [SerializeField] private OvenUI ovenWindow;
    [SerializeField] private SaucePanUI saucePanWindow;
    [SerializeField] private PanUI panWindow;

    [Header("Result Window")]
    [SerializeField] private GameObject resultWindow;
    [SerializeField] private Image resultImage;

    private CookingStation currentCookingStation;

    private float currentProgression;

    private CookStationData cookStationData;
    private GameObject cookStationObj;

    private DynamicUIListPoolable<FoodItem> itemsList;
    private string currentSubCategory;

    private List<(GameObject, FoodItem)> ingredients;
    private void Awake() {
        SetInstance(this);

        ingredients = new List<(GameObject, FoodItem)>();
        cookingStationWindow.SetActive(false);
        resultWindow.SetActive(false);
        SetActiveGameWindow(false);
        SetActiveConfirmCookWindow(false);
    }

    private void SetActiveStationWindow(bool state, CookStationData data = null) {
        cookingStationWindow.SetActive(state);

        if (state && data != null) {
            cookStationData = data;

            cuttingBoardImage.SetActive(false);
            mixerImage.SetActive(false);
            ovenImage.SetActive(false);
            panImage.SetActive(false);
            saucePanImage.SetActive(false);

            cookStationObj = data.Id switch {
                "CUTTING_BOARD" => cuttingBoardImage,
                "MIXER" => mixerImage,
                "OVEN" => ovenImage,
                "PAN" => panImage,
                "SAUCE_PAN" => saucePanImage,
                _ => null
            };

            if (cookStationObj == null) {
                throw new Exception($"Couldn't find a cooking station Image with id: {data.Id}");
            }

            cookStationObj.gameObject.SetActive(true);
            InventoryUIManager.Get().ShowInventory(inventory, OnClickItem, DataType.FoodData, false);
        }
    }

    public void ShowStationWindow(CookStationData data) {
        SetActiveStationWindow(true, data);
    }

    public void HideStationWindow() {
        SetActiveStationWindow(false);
        ClearIngredients();
    }

    private void SetActiveUnlockWindow(bool state, CookingStation cookingStation = null) {
        unlockWindow.SetActive(state);

        if (state && cookingStation != null) {
            unlockText.text = $"Are you sure, you want to unlock {cookingStation.GetCookingStationId()} for {CookingStation.UNLOCK_PRICE} ?";
            unlockPrice.text = CookingStation.UNLOCK_PRICE.ToString();
            unlockConfirmButton.interactable = InventoryManager.Get().GetMoney() >= CookingStation.UNLOCK_PRICE;
        }
    }

    public void ShowUnlockWindow(CookingStation cookingStation) {
        SetActiveUnlockWindow(true, cookingStation);
    }

    public void HideUnlockWindow() {
        SetActiveUnlockWindow(false);
    }

    public void OnClickConfirmUnlock() {
        currentCookingStation.UnlockStation();
        HideUnlockWindow();
    }

    private void SetActiveGameWindow(bool state, string id = "", List<FoodData> foods = null, int quality = 0) {
        HideGamesWindow();

        if (state && !string.IsNullOrWhiteSpace(id) && foods != null) {
            UpdateProgression(0);
            UpdateGameRecipe(foods);
            stationsGameWindowParent.SetActive(true);

            CookingStationUI stationUI = id switch {
                "CUTTING_BOARD" => cuttingBoardWindow,
                "MIXER" => mixerWindow,
                "OVEN" => ovenWindow,
                "PAN" => panWindow,
                "SAUCE_PAN" => saucePanWindow,
                _ => null
            };

            if (stationUI == null) {
                throw new Exception($"Couldn't find a cooking station UI with id: {id}");
            }

            stationUI.gameObject.SetActive(true);
            stationUI.StartGame(foods, quality);
        }
    }

    public void HideGamesWindow() {
        stationsGameWindowParent.SetActive(false);
        cuttingBoardWindow.gameObject.SetActive(false);
        mixerWindow.gameObject.SetActive(false);
        ovenWindow.gameObject.SetActive(false);
        panWindow.gameObject.SetActive(false);
        saucePanWindow.gameObject.SetActive(false);
    }

    private void UpdateGameRecipe(IEnumerable<FoodData> foods) {
        cookingStationRecipe.UpdateRecipe(foods.Select(food => food.Id).ToList(), cookStationData.Id);
    }

    private void OnClickItem(Item item) {
        AddIngredient(item as FoodItem);
    }

    #region COOKING
    private void AddIngredient(FoodItem item) {
        if (ingredients.Count <= cookStationData.InventorySize) {
            Transform itemsParent = cookStationObj.transform.GetChild(0);
            RectTransform parentRect = itemsParent.GetComponent<RectTransform>();
            Vector2 parentSize = parentRect.sizeDelta;
            Vector2 randomPosition = new (
                Random.Range(-parentSize.x * 0.4f, parentSize.x * 0.4f),
                Random.Range(-parentSize.y * 0.4f, parentSize.y * 0.4f)
            );
            GameObject ingredientObj = Instantiate(ingredientPrefab, itemsParent);
            (GameObject, FoodItem) ingredient = (ingredientObj, new FoodItem(item.Id, 1, item.Quality));
            ingredientObj.GetComponent<RectTransform>().anchoredPosition = randomPosition;
            ingredientObj.name = $"{item.Id}-{item.Quality}";
            ingredientObj.GetComponent<Image>().sprite = DataManager.GetIcon(item.Id);
            ingredientObj.GetComponentInChildren<Button>().onClick.AddListener(() => {
                RemoveIngredient(ingredient);
            });
            ingredients.Add(ingredient);
            InventoryManager.Get().RemoveItem(item, 1);

            string recipeHash = string.Join("-", ingredients.Select(i => i.Item2.Id).OrderBy(id => id)) + "-" + cookStationData.Id;
            UpdateCookingRecipe(ingredients.Select(i => i.Item2.Id).ToList());
            UpdateCookButtons(HasIngredients(), DataManager.GetFoodFromFoodRecipe(recipeHash));
        }
    }

    private void RemoveIngredient((GameObject obj, FoodItem item) ingredient, bool addToInventory = true) {
        if (addToInventory) {
            InventoryManager.Get().AddItem(ingredient.item);
        }

        ingredients.Remove(ingredient);
        Destroy(ingredient.obj);

        string recipeHash = string.Join("-", ingredients.Select(i => i.Item2.Id).OrderBy(id => id)) + "-" + cookStationData.Id;
        UpdateCookingRecipe(ingredients.Select(i => i.Item2.Id).ToList());
        UpdateCookButtons(HasIngredients(), DataManager.GetFoodFromFoodRecipe(recipeHash));
    }

    public void ClearIngredients(bool addToInventory = true) {
        foreach ((GameObject obj, FoodItem foodItem) in ingredients) {
            if (addToInventory) {
                InventoryManager.Get().AddItem(foodItem);
            }

            Destroy(obj);
        }
        ingredients.Clear();
        UpdateGameRecipe(null);
    }

    public bool HasIngredients() {
        return ingredients.Count > 0;
    }

    public void OnClickCook(bool skipConfirmation = false) {
        if (ingredients.Count <= 0) {
            Debug.Log("Try to cook without any ingredients !");
            return;
        }

        string recipeHash = string.Join("-", ingredients.Select(ingredient => ingredient.Item2.Id).OrderBy(id => id)) + "-" + cookStationData.Id;
        int ingredientsQuality = ingredients.Select(ingredient => ingredient.Item2.Quality).Sum() / ingredients.Count;
        FoodData resultData = DataManager.GetFoodFromFoodRecipe(recipeHash);
        if (!skipConfirmation && (resultData.Id == "UNKNOWN_MIXTURE" || !BookManager.Get().HasRecipe(resultData.Id))) {
            SetActiveConfirmCookWindow(true);
        } else {
            SetActiveGameWindow(true, cookStationData.Id, ingredients.Select(ingredient => ingredient.Item2.Data as FoodData).ToList(), ingredientsQuality);
            ClearIngredients(false);
            UpdateGameRecipe(null);
            UpdateCookButtons(false, null);
        }
    }

    private void SetActiveConfirmCookWindow(bool state) {
        confirmationCookWindow.SetActive(state);
    }

    public void OnClickConfirmCookYes() {
        SetActiveConfirmCookWindow(false);
        OnClickCook(true);
    }

    public void OnClickConfirmCookNo() {
        SetActiveConfirmCookWindow(false);
    }

    public void OnClickAutoCook() {
        if (ingredients.Count <= 0) {
            Debug.Log("Try to cook without any ingredients!");
            return;
        }

        string recipeHash = string.Join("-", ingredients.Select(ingredient => ingredient.Item2.Id).OrderBy(id => id)) + "-" + cookStationData.Id;
        FoodData resultData = DataManager.GetFoodFromFoodRecipe(recipeHash);
        if (!IsAutoCookable(resultData)) {
            Debug.Log("Try to auto-cook but it's not possible for this recipe!");
        } else {
            //TODO Little animation on click to show item was cooked
            int ingredientsQuality = ingredients.Select(ingredient => ingredient.Item2.Quality).Sum() / ingredients.Count;
            InventoryManager.Get().AddItem(new FoodItem(resultData.Id, 1, ingredientsQuality));
            ClearIngredients(false);
            AddIngredients(resultData.GetRecipe());
        }
    }

    private bool IsAutoCookable(FoodData foodData) {
        return (foodData.Id != "UNKNOWN_MIXTURE" && BookManager.Get().HasRecipe(foodData.Id) && BookManager.Get().GetCookCount(foodData.Id) >= foodData.NeededCookForAutoCook);
    }

    public void OnClickRecipeBook() {
        BookUIManager.Get().ShowBook(OnClickRecipe, BookUIManager.BookCategoryType.FOOD, false, cookStationData.Id, false);
    }

    private void OnClickRecipe(RecipeData data) {
        ClearIngredients();
        AddIngredients(data.RecipeHash);
        BookUIManager.Get().HideBook();
    }

    private void AddIngredients(string recipeHash) {
        string[] recipe = recipeHash.Split("-").SkipLast(1).ToArray();
        foreach (string ingredient in recipe) {
            FoodItem item = InventoryManager.Get().GetItem<FoodItem>(ingredient);
            if (item != null) {
                AddIngredient(item);
            }
        }
    }

    private void UpdateCookingRecipe(List<string> foods) {
        gameRecipe.UpdateRecipe(foods, cookStationData.Id);
    }

    private void UpdateCookButtons(bool hasIngredients, FoodData result) {
        cookButton.interactable = hasIngredients;
        bool isAutoCookable = result != null && IsAutoCookable(result);
        bool isKnownRecipe = result != null && BookManager.Get().HasRecipe(result.Id);
        autoCookButton.interactable = hasIngredients && isAutoCookable;
        autoCookInfo.SetActive(hasIngredients && isKnownRecipe && !isAutoCookable);
        if (autoCookInfo.activeInHierarchy) {
            autoCookText.text = $"{BookManager.Get().GetCookCount(result.Id)}/{result.NeededCookForAutoCook}";
        }
    }

    private void UpdateProgression(float value) {
        currentProgression = Mathf.Clamp(value, 0, 100);
        gameProgressionText.text = $"{currentProgression:F0}%";
        gameProgressionSlider.value = currentProgression;
    }

    public void IncrementProgression(float add) {
        UpdateProgression(currentProgression + add);
    }

    public void SetProgression(float value) {
        UpdateProgression(value);
    }
    #endregion

    #region RESULT
    private void SetActiveResultWindow(bool state, FoodItem item = null) {
        resultWindow.SetActive(state);
        if (state && item != null) {
            resultImage.sprite = DataManager.GetIcon(item.Id);
            for (int count = 0; count < FoodItem.MAX_QUALITY; count++) {
                resultImage.transform.Find("Quality").GetChild(count).gameObject.SetActive(count + 1 == item.Quality);
            }
        }
    }

    public void ShowCookingResult(FoodItem item) {
        SetActiveResultWindow(true, item);
    }

    public void HideCookingResult() {
        SetActiveResultWindow(false);
    }

    public void OnClickOkButton() {
        HideCookingResult();
    }
    #endregion
}