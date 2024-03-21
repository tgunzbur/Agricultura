using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Agricultura.Data {
    public class DataManager : MonoSingleton<DataManager> {
        public static readonly JsonSerializerSettings JsonSettings = new () { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };

        public Sprite MoneyIcon;
        public Sprite UnknownIcon;

        [Header("Item Background Rarities")]
        [SerializeField] private Sprite NoRarity;
        [SerializeField] private Sprite CommonRarity;
        [SerializeField] private Sprite UncommonRarity;
        [SerializeField] private Sprite RareRarity;
        [SerializeField] private Sprite MythicRarity;
        [SerializeField] private Sprite LegendaryRarity;

        public List<TextAsset> jsonFiles;
        public List<Sprite> sprites;

        private static string SAVE_PATH;

        private static Dictionary<string, BaseData> dataDictionary = new();
        private static Dictionary<string, Sprite> icons = new ();
        private static Dictionary<string, List<PlantData>> plantsRecipes = new ();
        private static Dictionary<string, FoodData> foodsRecipes = new ();
        private static Dictionary<string, PlantData> foodsPlant = new ();

        private void Awake() {
            SetInstance(this);
            SAVE_PATH = Application.persistentDataPath + "/save.json";

            LoadAllFiles();
            for (int count = 0; count < dataDictionary.Count; count++) {
                icons[jsonFiles[count].name.Trim()] = sprites[count];
            }
        }

        public Sprite GetBackgroundItemRarity(int rarity) {
            return rarity switch {
                1 => CommonRarity,
                2 => UncommonRarity,
                3 => RareRarity,
                4 => MythicRarity,
                5 => LegendaryRarity,
                _ => NoRarity
            };
        }

        public static PlantData GetPlantFromFood(string foodId) {
            return foodsPlant.ContainsKey(foodId) ? foodsPlant[foodId] : null;
        }

        public static List<PlantData> GetPlantsFromRecipe(string recipeHash) {
            if (string.IsNullOrWhiteSpace(recipeHash) || !plantsRecipes.ContainsKey(recipeHash)) {
                return null;
            }
            return plantsRecipes[recipeHash];
        }

        public static FoodData GetFoodFromFoodRecipe(string recipeHash) {
            if (string.IsNullOrWhiteSpace(recipeHash) || !foodsRecipes.ContainsKey(recipeHash)) {
                return GetData<FoodData>("UNKNOWN_MIXTURE");
            }
            return foodsRecipes[recipeHash];
        }

        public static List<FoodData> GetIngredientsFromFoodRecipe(string recipeHash) {
            if (string.IsNullOrWhiteSpace(recipeHash)) {
                return null;
            }

            return recipeHash.Split("-").SkipLast(1).Select(GetData<FoodData>).ToList();
        }

        public static Sprite GetIcon(string id) {
            if (string.IsNullOrWhiteSpace(id) || !icons.ContainsKey(id)) {
                throw new Exception($"Sprite for \"{id}\" doesn't exist");
            }
            return icons[id];
        }

        public static T GetData<T>(string id) where T : BaseData {
            if (dataDictionary != null && dataDictionary.ContainsKey(id)) {
                return dataDictionary[id] as T;
            }
            throw new Exception($"Data for {id} not found");
        }

        public static List<T> GetAll<T>(DataType type) where T : BaseData {
            if (dataDictionary != null) {
                return dataDictionary.Values.Where(data => data.Type == type).Select(data => data as T).ToList();
            }
            throw new Exception($"Data of type {type} not found");
        }

        private void LoadAllFiles() {
            foreach (TextAsset textAsset in jsonFiles) {
                BaseData data = JsonConvert.DeserializeObject<BaseData>(textAsset.text, JsonSettings);
                if (data == null) {
                    Debug.LogError($"Data from file \"{textAsset.name}\" doesn't exist");
                    continue;
                }

                dataDictionary[data.Id] = data;
                if (data.Type == DataType.PlantData) {
                    LoadPlantData(data as PlantData);
                } else if (data.Type == DataType.FoodData) {
                    LoadFoodData(data as FoodData);
                }
            }
        }

        private static void LoadPlantData(PlantData plantData) {
            string foodId = plantData.ResultFoodId;
            if (!foodsPlant.ContainsKey(foodId)) {
                foodsPlant[foodId] = plantData;
            } else {
                Debug.LogError($"{foodsPlant[foodId].Id} and {plantData.Id} have the same food as result ({foodId}) !");
            }

            string recipeHash = plantData.GetRecipe();
            if (string.IsNullOrWhiteSpace(recipeHash)) {
                return;
            }
            if (!plantsRecipes.ContainsKey(recipeHash)) {
                plantsRecipes[recipeHash] = new List<PlantData>();
            }
            plantsRecipes[recipeHash].Add(plantData);
        }

        private static void LoadFoodData(FoodData foodData) {
            string recipeHash = foodData.GetRecipe();
            if (string.IsNullOrWhiteSpace(recipeHash)) {
                return;
            }
            if (!foodsRecipes.ContainsKey(recipeHash)) {
                foodsRecipes[recipeHash] = foodData;
            } else {
                Debug.LogError($"{foodsRecipes[recipeHash]} and {foodData.Id} have the same recipe ({recipeHash}) !");
            }
        }

        public static SerializedGame LoadSave() {
            if (File.Exists(SAVE_PATH)) {
                string text = File.ReadAllText(SAVE_PATH);
                return JsonConvert.DeserializeObject<SerializedGame>(text, JsonSettings);
            }

            return null;
        }

        public static void Save(SerializedGame game) {
            string text = JsonConvert.SerializeObject(game, Formatting.Indented, JsonSettings);
            File.WriteAllText(SAVE_PATH, text);
        }

        public static void DeleteSave() {
            File.Delete(SAVE_PATH);
        }
    }
}