using Agricultura.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class DropUIManager : MonoSingleton<DropUIManager> {
        [SerializeField] DroppedItem droppedItemPrefab;

        private void Awake() {
            SetInstance(this);
        }

        public void OnDropItem(Transform parent, Item item) {
            DroppedItem droppedItem = Instantiate(droppedItemPrefab, parent);
            droppedItem.GetComponent<Image>().sprite = item.Data.GetIcon();
            droppedItem.OnCompleted += () => InventoryManager.Get().AddItem(item);
        }

        public void OnDropMoney(Transform parent, int money) {
            DroppedItem droppedItem = Instantiate(droppedItemPrefab, parent);
            droppedItem.GetComponent<Image>().sprite = DataManager.Get().MoneyIcon;
            droppedItem.OnCompleted += () => InventoryManager.Get().AddMoney(money);
        }
    }
}