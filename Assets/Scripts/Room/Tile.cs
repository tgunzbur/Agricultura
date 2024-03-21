using Agricultura;
using UnityEngine;
using UnityEngine.UI;

public static class Direction {
    public const int LeftId = 0;
    public const int UpId = 1;
    public const int RightId = 2;
    public const int DownId = 3;
}

public abstract class Tile : MonoBehaviour {
    public Position Position {
        get;
        private set;
    }

    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => OnClick(InventoryManager.Get().GetCurrentItem()));
    }

    public void Init(Position position) {
        Position = position;
    }

    protected abstract bool OnClick(Item item);
}