using UnityEngine;

public class Room<T> : MonoBehaviour where T : Tile {
    [SerializeField] protected T tilePrefab;

    protected const int ROOM_SIZE = 5;

    protected T[,] tiles;

    private void Awake() {
        Create();
    }

    public void Regenerate() {
        foreach (T tile in tiles) {
            Destroy(tile.gameObject);
        }
        Create();
    }

    private void Create() {
        tiles = new T[ROOM_SIZE, ROOM_SIZE];

        Position position = Position.Zero;
        for (position.y = 0; position.y < ROOM_SIZE; position.y++) {
            for (position.x = 0; position.x < ROOM_SIZE; position.x++) {
                tiles[position.x, position.y] = CreateTile(position, tilePrefab);
            }
        }
    }

    protected virtual T CreateTile(Position position, T prefab) {
        T tile = Instantiate(prefab, transform);
        tile.name = $"Tile_{position.x}_{position.y}";
        tile.Init(new Position(position));

        return tile;
    }

    public T GetTileAt(Position position) {
        if (position.x >= 0 && position.x < ROOM_SIZE && position.y >= 0 && position.y < ROOM_SIZE) {
            return tiles[position.x, position.y];
        }

        return null;
    }

    public T[,] GetTiles() {
        return tiles;
    }
}