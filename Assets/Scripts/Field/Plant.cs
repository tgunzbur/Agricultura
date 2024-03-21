using System;
using System.Collections;
using System.Collections.Generic;
using Agricultura.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agricultura {
    public class Plant : MonoBehaviour {
        public string Id => data.Id;
        public bool IsFullyGrown => Growth >= 1;

        public float Growth {
            get;
            private set;
        }
        private PlantData data;

        private static readonly float[] chanceByQualities = new float[3] {
            0.9f,
            0.075f,
            0.025f
        };

        private static readonly float[] chanceByQuantities = new float[3] {
            0.4f,
            0.35f,
            0.25f
        };

        private void FixedUpdate() {
            if (IsFullyGrown || !TimeManager.Get().TimePass) {
                return;
            }
            SetGrowth(Growth + Time.fixedDeltaTime / data.GrowthTime);
        }

        public void SetData(PlantData newData) {
            if (data != null) {
                throw new Exception("Can't change Plant data when it was already set");
            }

            data = newData;
        }

        public void SetGrowth(float newGrowth) {
            Growth = newGrowth;
            transform.localScale = Vector3.one * (Growth * 0.8f + 0.1f);
        }

        private int GetRandomQuantity() {
            float random = Random.value;
            float totalChance = 0;
            for (int index = 0; index < chanceByQuantities.Length; index++) {
                totalChance += chanceByQuantities[index];
                if (random <= totalChance) {
                    return index + 1;
                }
            }

            throw new Exception("Total of quantities chance in Plant must be 1");
        }

        private int GetRandomQuality() {
            float random = Random.value;
            float totalChance = 0;
            for (int index = 0; index < chanceByQualities.Length; index++) {
                totalChance += chanceByQualities[index];
                if (random <= totalChance) {
                    return index + 1;
                }
            }

            throw new Exception("Total of qualities chance in Plant must be 1");
        }

        public void OnCollect() {
            if (IsFullyGrown) {
                FoodItem foodItem = new (data.GetFoodResult(), GetRandomQuantity(), GetRandomQuality());
                DropUIManager.Get().OnDropItem(transform.parent, foodItem);
            } else if (Random.value <= Growth) {
                PlantItem plantItem = new (data, 1);
                DropUIManager.Get().OnDropItem(transform.parent, plantItem);
            }

            Destroy(gameObject);
        }

        private const float CHANCE_TO_GROW_SAME_SEED = 0.025f;
        public static Dictionary<string, float> GetCreatablePlantsFactor(List<Plant> plants) {
            Dictionary<string, float> result = new ();
            foreach (string plantRecipeHash in GenerateRecipeHashSubset(plants)) {
                List<PlantData> plantsResult = DataManager.GetPlantsFromRecipe(plantRecipeHash);
                if (plantsResult == null) {
                    continue;
                }
                foreach (PlantData plant in plantsResult) {
                    if (!result.TryAdd(plant.Id, 1f / plant.Rarity)) {
                        result[plant.Id] += 1f / plant.Rarity;
                    }
                }
            }

            foreach (Plant plant in plants) {
                if (!result.TryAdd(plant.Id, CHANCE_TO_GROW_SAME_SEED / plant.data.Rarity)) {
                    result[plant.Id] += CHANCE_TO_GROW_SAME_SEED / plant.data.Rarity;
                }
            }

            return result;
        }

        public static List<string> GenerateRecipeHashSubset(List<Plant> plants) {
            List<string> result = new ();

            int length = plants.Count;
            for (int i = 1; i < (1 << length); i++) {
                string hash = "";
                for (int j = 0; j < length; j++) {
                    if ((i & (1 << j)) > 0) {
                        if (hash == "") {
                            hash = plants[j].Id;
                        } else {
                            hash += "-" + plants[j].Id;
                        }
                    }
                }
                result.Add(hash);
            }

            return result;
        }

        #region TUTORIAL
        public void FullyGrow(float time = 2.5f) {
            StartCoroutine(FullyGrowCoroutine(time));
        }


        private IEnumerator FullyGrowCoroutine(float time) {
            while (Growth < 1) {
                SetGrowth(Growth + Time.deltaTime / time);
                yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}