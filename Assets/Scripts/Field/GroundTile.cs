using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Agricultura.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Agricultura {
    public class GroundTile : Tile {
        public Plant Plant {
            get;
            private set;
        }

        public readonly GroundTile[] Neighbours = new GroundTile[4];
        private const float chanceToGrowAPlant = 0.0000025f;
        private float currentChanceToGrowAPlant = chanceToGrowAPlant;
        private void FixedUpdate() {
            if (Plant != null || !TimeManager.Get().TimePass) {
                return;
            }

            List<Plant> orderedPlantList = Neighbours.
                Where(ground => ground != null && ground.Plant != null && ground.Plant.IsFullyGrown).
                Select(ground => ground.Plant).OrderBy(plant => plant.Id, new StringComparer()).ToList();
            string seedId = GetPlantToCreate(orderedPlantList);
            if (seedId != null) {
                AddPlant(seedId);
            }
        }

        protected override bool OnClick(Item item) {
            if (item == null) {
                return false;
            }

            if (Plant != null && item is ScytheItem) {
                RemovePlant();
                return true;
            }

            if (Plant != null || item is not PlantItem plantItem) {
                ErrorUIManager.Get().ShowForbiddenError(transform);
                return false;
            }

            AddPlant(plantItem.Id);
            InventoryManager.Get().RemoveItem(plantItem, 1);
            return true;
        }

        private string GetPlantToCreate(List<Plant> plants) {
            if (plants.Count == 0) {
                return null;
            }

            Dictionary<string, float> plantsFactor = Plant.GetCreatablePlantsFactor(plants);
            float random = Random.value * plantsFactor.Values.Sum() / currentChanceToGrowAPlant;
            float count = 0;
            foreach (string plantId in plantsFactor.Keys) {
                count += plantsFactor[plantId];
                if (random <= count) {
                    return plantId;
                }
            }

            if (currentChanceToGrowAPlant < chanceToGrowAPlant * 100) {
                currentChanceToGrowAPlant += chanceToGrowAPlant / 10;
            }
            return null;
        }

        public void AddPlant(string plantId, float startGrowth = 0) {
            if (Plant != null) {
                throw new Exception("Try to plant on a tile with a current seed !");
            }

            Plant = PlantFactory.Get().CreatePlantAt(this, plantId);
            Plant.SetGrowth(startGrowth);

            currentChanceToGrowAPlant = chanceToGrowAPlant;
        }

        public void AddPlant(SerializedPlant plant) {
            AddPlant(plant.Id, plant.Growth);
        }

        private void RemovePlant() {
            if (Plant == null) {
                throw new Exception("Try to remove a seed from a tile without a current seed !");
            }
            Plant.OnCollect();
            Plant = null;
        }

        private class StringComparer : IComparer<string> {
            public int Compare(string a, string b) {
                return string.Compare(a, b, true, CultureInfo.InvariantCulture);
            }
        }
    }
}