using System;
using Agricultura.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class PlantFactory : MonoSingleton<PlantFactory> {
        [SerializeField] private Plant plantPrefab;

        private void Awake() {
            SetInstance(this);
        }

        public Plant CreatePlantAt(GroundTile tile, string plantId) {
            return CreatePlantAt(tile, DataManager.GetData<PlantData>(plantId));
        }

        private Plant CreatePlantAt(GroundTile tile, PlantData data) {
            if (data == null) {
                throw new Exception("Can't create plant with null data");
            }

            Plant plant = Instantiate(plantPrefab, tile.transform);
            plant.GetComponent<Image>().sprite = data.GetFoodResult().GetIcon();
            plant.SetData(data);

            return plant;
        }
    }
}