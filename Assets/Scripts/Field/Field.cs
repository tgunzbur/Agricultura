using System.Collections.Generic;
using Agricultura.Data;

namespace Agricultura {
    public class Field : Room<GroundTile> {
        protected override GroundTile CreateTile(Position position, GroundTile prefab) {
            GroundTile tile = base.CreateTile(position, prefab);

            if (position.x > 0) {
                tile.Neighbours[Direction.LeftId] = tiles[position.x - 1, position.y];
                tiles[position.x - 1, position.y].Neighbours[Direction.RightId] = tile;
            }
            if (position.y > 0) {
                tile.Neighbours[Direction.DownId] = tiles[position.x, position.y - 1];
                tiles[position.x, position.y - 1].Neighbours[Direction.UpId] = tile;
            }

            return tile;
        }

        public void FromSerialized(SerializedField serializedField) {
            foreach (SerializedPlant plant in serializedField.Plants) {
                GetTileAt(plant.Position).AddPlant(plant);
            }
        }

        public SerializedField ToSerialized() {
            SerializedField serializedField = new ();
            foreach (GroundTile tile in GetTiles()) {
                if (tile.Plant != null) {
                    serializedField.Plants.Add(new SerializedPlant(tile.Position, tile.Plant));
                }
            }
            return serializedField;
        }
    }
}