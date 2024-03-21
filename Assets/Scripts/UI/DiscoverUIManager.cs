using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class DiscoverUIManager : MonoSingleton<DiscoverUIManager> {
        [SerializeField] private Transform notificationsParent;
        [SerializeField] private NotificationUI notificationPrefab;

        private void Awake() {
            SetInstance(this);
        }

        private void Start() {
            InventoryManager.Get().OnAddItem += OnDiscoverItem;
        }

        private void OnDiscoverItem(Item item) {
            if (!DiscoveriesManager.Get().AddDiscovery(item.Id)) {
                return;
            }

            NotificationUI notification = Instantiate(notificationPrefab, notificationsParent);
            notification.UpdateNotification(item.Data);
        }
    }
}