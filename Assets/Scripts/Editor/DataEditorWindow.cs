using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Agricultura.Data;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor {
    public class DataEditorWindow : EditorWindow {
        private BaseData currentData;

        internal static void ShowWindow(BaseData data) {
            DataEditorWindow window = GetWindow<DataEditorWindow>();
            window.Init(data);
        }

        private void Init(BaseData data) {
            currentData = data;
        }

        private static Vector2 currentScrollPosition = Vector2.zero;
        private void OnGUI() {
            if (currentData == null) {
                return;
            }
            currentScrollPosition = EditorGUILayout.BeginScrollView(currentScrollPosition, false, true);

            Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(currentData.IconPath, typeof(Sprite));
            sprite = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Icon:"), sprite, typeof(Sprite), false);
            if (sprite != null) {
                currentData.IconPath = AssetDatabase.GetAssetPath(sprite);
            }

            currentData.Id = EditorGUILayout.TextField("Id:", currentData.Id);
            currentData.Name = EditorGUILayout.TextField("Name:", currentData.Name);
            currentData.Description = EditorGUILayout.TextField("Description:", currentData.Description);
            currentData.Price = EditorGUILayout.IntField("Price:", currentData.Price);

            if (currentData is PlantData plantData) {
                ShowPlantDataParameters(plantData);
            } else if (currentData is FoodData foodData) {
                ShowFoodDataParameters(foodData);
            } else if (currentData is CookStationData cookStationData) {
                ShowCookStationDataParameters(cookStationData);
            } else if (currentData is StallData stallData) {
                ShowIAStallDataParameters(stallData);
            }

            if (GUILayout.Button("Save")) {
                DataFileListEditorWindow.Save(currentData);
            }
            EditorGUILayout.EndScrollView();
        }

        private void ShowPlantDataParameters(PlantData plantData) {
            plantData.Rarity = EditorGUILayout.IntField("Rarity (1-5):" , plantData.Rarity);
            plantData.GrowthTime = EditorGUILayout.IntField("Growth time (seconds):" , plantData.GrowthTime);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Result:");
            if (EditorGUILayout.DropdownButton(new GUIContent(plantData.ResultFoodId), FocusType.Passive)) {
                GenericMenu menu = new ();
                foreach (string foodId in DataFileListEditorWindow.GetAllData(DataType.FoodData)) {
                    menu.AddItem(new GUIContent(foodId), foodId == plantData.ResultFoodId, () => {
                        plantData.ResultFoodId = foodId;
                    });
                }
                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label("Recipe:");
            for (int plantCount = 0; plantCount < plantData.Recipe.Count; plantCount++) {
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayout.DropdownButton(new GUIContent(plantData.Recipe[plantCount]), FocusType.Passive)) {
                    GenericMenu menu = new ();
                    foreach (string plantId in DataFileListEditorWindow.GetAllData(DataType.PlantData)) {
                        int tmpPlantCount = plantCount;
                        menu.AddItem(new GUIContent(plantId), plantId == plantData.Recipe[plantCount], () => {
                            plantData.Recipe[tmpPlantCount] = plantId;
                        });
                    }
                    menu.ShowAsContext();
                }

                if (GUILayout.Button("-")) {
                    plantData.Recipe.RemoveAt(plantCount--);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+")) {
                plantData.Recipe.Add(DataFileListEditorWindow.GetAllData(DataType.PlantData)?[0] ?? "");
            }
        }

        private void ShowFoodDataParameters(FoodData foodData) {
            GUILayout.Label("Recipe:");
            for (int foodCount = 0; foodCount < foodData.RecipeIngredients.Count; foodCount++) {
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayout.DropdownButton(new GUIContent(foodData.RecipeIngredients[foodCount]), FocusType.Passive)) {
                    GenericMenu menu = new ();
                    foreach (string foodId in DataFileListEditorWindow.GetAllData(DataType.FoodData)) {
                        int tmpFoodCount = foodCount;
                        menu.AddItem(new GUIContent(foodId), foodId == foodData.RecipeIngredients[foodCount], () => {
                            foodData.RecipeIngredients[tmpFoodCount] = foodId;
                        });
                    }
                    menu.ShowAsContext();
                }

                if (GUILayout.Button("-")) {
                    foodData.RecipeIngredients.RemoveAt(foodCount--);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+")) {
                foodData.RecipeIngredients.Add(DataFileListEditorWindow.GetAllData(DataType.FoodData)?[0] ?? "");
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Furniture:");
            if (EditorGUILayout.DropdownButton(new GUIContent(foodData.RecipeFurniture), FocusType.Passive)) {
                GenericMenu menu = new ();
                foreach (string furnitureId in DataFileListEditorWindow.GetAllData(DataType.HouseFurnitureData)) {
                    menu.AddItem(new GUIContent(furnitureId), furnitureId == foodData.RecipeFurniture, () => {
                        foodData.RecipeFurniture = furnitureId;
                    });
                }
                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowCookStationDataParameters(CookStationData cookStationData) {
            cookStationData.NeededClicks = EditorGUILayout.IntField("Needed clicks:", cookStationData.NeededClicks);
            cookStationData.InventorySize = EditorGUILayout.IntField("Inventory size:", cookStationData.InventorySize);
        }

        private void ShowIAStallDataParameters(StallData stallData) {
            GUILayout.Label($"Sellable items:");
            for (int itemCount = 0; itemCount < stallData.SellableItems.Count; itemCount++) {
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayout.DropdownButton(new GUIContent(stallData.SellableItems[itemCount]), FocusType.Passive)) {
                    GenericMenu menu = new ();
                    foreach (string itemId in DataFileListEditorWindow.GetAllData()) {
                        int tmpItemCount = itemCount;
                        menu.AddItem(new GUIContent(itemId), itemId == stallData.SellableItems[tmpItemCount], () => {
                            stallData.SellableItems[tmpItemCount] = itemId;
                        });
                    }
                    menu.ShowAsContext();
                }
                if (GUILayout.Button("-")) {
                    stallData.SellableItems.RemoveAt(itemCount--);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+")) {
                stallData.SellableItems.Add(DataFileListEditorWindow.GetAllData()?[0] ?? "");
            }
        }
    }

    public class DataFileListEditorWindow : EditorWindow {
        private string nameFilter;
        private string idData;
        private DataType typeData;
        private Vector2 currentScrollPosition;

        private static List<BaseData> currentDataList;

        [MenuItem ("Window/DataEditor")]
        public static void ShowWindow() {
            currentDataList = LoadAllDataFiles();
            GetWindow(typeof(DataFileListEditorWindow));
        }

        private void OnGUI() {
            Texture2D lightBackground = new (1, 1);
            lightBackground.SetPixel(0, 0, new Color(99,102,106));
            GUIStyle lightStyle = new ();
            lightStyle.normal.background = lightBackground;

            if (GUILayout.Button("Refresh") || currentDataList == null) {
                currentDataList = LoadAllDataFiles();
            }

            GUILayout.Label("Data List:", EditorStyles.boldLabel);
            currentScrollPosition = EditorGUILayout.BeginScrollView(currentScrollPosition, false, true);
            for (int count = 0; count < currentDataList.Count; count++) {
                BaseData data = currentDataList[count];

                if (count % 2 == 0) {
                    EditorGUILayout.BeginHorizontal(lightStyle, GUILayout.Width(position.width * 0.95f));
                    EditorGUILayout.LabelField(data.Name, lightStyle, GUILayout.Width(position.width * 0.5f));
                    EditorGUILayout.LabelField(data.Type.ToString().Replace("Data", ""), lightStyle, GUILayout.Width(position.width * 0.25f));
                } else {
                    EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width * 0.95f));
                    EditorGUILayout.LabelField(data.Name, GUILayout.Width(position.width * 0.5f));
                    EditorGUILayout.LabelField(data.Type.ToString().Replace("Data", ""), GUILayout.Width(position.width * 0.25f));
                }

                if (GUILayout.Button("Open", GUILayout.Width(position.width * 0.1f))) {
                    DataEditorWindow.ShowWindow(data as BaseData);
                }
                if (GUILayout.Button("Delete", GUILayout.Width(position.width * 0.1f))) {
                    Delete(data);
                    currentDataList.RemoveAt(count--);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            idData = EditorGUILayout.TextField("Id:", idData);
            typeData = (DataType)EditorGUILayout.EnumPopup(typeData);
            HouseFurnitureDataType houseSubType = HouseFurnitureDataType.Default;
            if (typeData == DataType.HouseFurnitureData) {
                houseSubType = (HouseFurnitureDataType)EditorGUILayout.EnumPopup(houseSubType);
            }
            if (GUILayout.Button("Create") && !string.IsNullOrEmpty(idData) && !File.Exists(GetFullPath(typeData, idData))) {
                BaseData data = typeData switch {
                    DataType.BaseData => new BaseData(idData),
                    DataType.PlantData => new PlantData(idData),
                    DataType.FoodData => new FoodData(idData),
                    DataType.HouseFurnitureData => houseSubType switch {
                        HouseFurnitureDataType.Default => new HouseFurnitureData(idData),
                        HouseFurnitureDataType.CookStationData => new CookStationData(idData),
                        _ => null
                    },
                    DataType.StallData => new StallData(idData),
                    _ => null
                };
                Save(data);
                currentDataList.Add(data);
                DataEditorWindow.ShowWindow(data);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Save All")) {
                DataManager dataManager = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(DATA_MANAGER_PATH, typeof(GameObject))).GetComponent<DataManager>();;
                dataManager.jsonFiles.Clear();
                dataManager.sprites.Clear();
                PrefabUtility.SaveAsPrefabAsset(dataManager.gameObject, DATA_MANAGER_PATH);
                DestroyImmediate(dataManager.gameObject);

                foreach (BaseData data in currentDataList) {
                    Save(data);
                }
            }
        }

        private static List<BaseData> LoadAllDataFiles() {
            AssetDatabase.Refresh();
            List<BaseData> dataList = new ();

            if (!Directory.Exists(GetRootPath())) {
                Directory.CreateDirectory(GetRootPath());
            }
            foreach (DataType type in Enum.GetValues(typeof(DataType))) {
                if (!Directory.Exists(GetFolderPath(type))) {
                    Directory.CreateDirectory(GetFolderPath(type));
                }
                foreach (string filePath in Directory.EnumerateFiles(GetFolderPath(type), "*.json")) {
                    BaseData data = JsonConvert.DeserializeObject<BaseData>(File.ReadAllText(filePath), DataManager.JsonSettings);
                    if (data != null) {
                        dataList.Add(data);
                    }
                }
            }

            return dataList;
        }

        internal static List<string> GetAllData() {
            return currentDataList.Select(data => data.Id).ToList();
        }

        internal static List<string> GetAllData(DataType dataType) {
            return (dataType switch {
                DataType.PlantData => currentDataList.Where(data => data.Type == DataType.PlantData),
                DataType.FoodData => currentDataList.Where(data => data.Type == DataType.FoodData),
                DataType.HouseFurnitureData => currentDataList.Where(data => data.Type == DataType.HouseFurnitureData),
                DataType.StallData => currentDataList.Where(data => data.Type == DataType.StallData),
                _ => null
            })?.Select(data => data.Id).ToList();
        }

        private const string DATA_MANAGER_PATH = "Assets/Prefabs/DataManager.prefab";
        internal static void Save(BaseData data) {
            if (!Directory.Exists(GetRootPath())) {
                Directory.CreateDirectory(GetRootPath());
            }
            if (!Directory.Exists(GetFolderPath(data))) {
                Directory.CreateDirectory(GetFolderPath(data));
            }
            File.WriteAllText(GetFullPath(data), JsonConvert.SerializeObject(data, DataManager.JsonSettings));

            DataManager dataManager = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(DATA_MANAGER_PATH, typeof(GameObject))).GetComponent<DataManager>();
            TextAsset file = AssetDatabase.LoadAssetAtPath<TextAsset>(GetRelativePath(data));
            if (!dataManager.jsonFiles.Contains(file)) {
                dataManager.jsonFiles.Add(file);
                dataManager.sprites.Add(AssetDatabase.LoadAssetAtPath<Sprite>(data.IconPath));
            } else {
                int index = dataManager.jsonFiles.FindIndex(textAsset => textAsset.Equals(file));
                dataManager.sprites[index] = AssetDatabase.LoadAssetAtPath<Sprite>(data.IconPath);
            }
            PrefabUtility.SaveAsPrefabAsset(dataManager.gameObject, DATA_MANAGER_PATH);
            DestroyImmediate(dataManager.gameObject);
        }

        private static void Delete(BaseData data) {
            File.Delete(GetFullPath(data));
            File.Delete(GetFullPath(data) + ".meta");
        }

        private static string GetRootPath() {
            return $"{Application.dataPath}/Data";
        }

        private static string GetFolderPath(BaseData data) {
            return GetFolderPath(data.Type);
        }

        private static string GetFolderPath(DataType type) {
            return $"{GetRootPath()}/{type.ToString().Replace("Data", "")}";
        }

        private static string GetFullPath(BaseData data) {
            return GetFullPath(data.Type, data.Id);
        }

        private static string GetFullPath(DataType type, string id) {
            return $"{GetFolderPath(type)}/{id}.json";
        }

        private static string GetRelativePath(BaseData data) {
            return $"Assets/Data/{data.Type.ToString().Replace("Data", "")}/{data.Id}.json";
        }
    }
}