using System;
using UnityEngine;

namespace Agricultura {
    public class MapUIManager : MonoSingleton<MapUIManager>  {
        [Header("Window Parameters")]
        [SerializeField] private RectTransform fieldWindow;
        [SerializeField] private RectTransform houseWindow;
        [SerializeField] private RectTransform marketWindow;

        [Header("Map Parameters")]
        [SerializeField] private GameObject mapWindow;

        public Action<string> OnSwitchWindow;
        private void Awake() {
            SetInstance(this);
            fieldWindow.anchoredPosition = Vector2.zero;
            houseWindow.anchoredPosition = Vector2.zero;
            marketWindow.anchoredPosition = Vector2.zero;
            OnClickField();
        }

        private void SwitchWindow(RectTransform targetWindow) {
            targetWindow.SetSiblingIndex(targetWindow.parent.childCount);
        }

        public void OnClickMap() {
            mapWindow.SetActive(true);
        }

        public void OnCloseMap() {
            mapWindow.SetActive(false);
        }

        public void OnClickHouse() {
            SwitchWindow(houseWindow);
            OnCloseMap();
            OnSwitchWindow?.Invoke("HOUSE");
        }

        public void OnClickField() {
            SwitchWindow(fieldWindow);
            OnCloseMap();
            OnSwitchWindow?.Invoke("FIELD");
        }

        public void OnClickMarket() {
            SwitchWindow(marketWindow);
            OnCloseMap();
            OnSwitchWindow?.Invoke("MARKET");
        }
    }
}