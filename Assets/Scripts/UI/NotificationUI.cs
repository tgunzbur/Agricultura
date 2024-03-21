using Agricultura.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class NotificationUI : MonoBehaviour {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;

        public void UpdateNotification(BaseData data) {
            icon.sprite = data.GetIcon();
            text.text = $"Discover {data.Name}!";
        }
    }
}