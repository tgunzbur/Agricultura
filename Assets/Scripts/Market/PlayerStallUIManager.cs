using System.Collections.Generic;
using Agricultura.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class PlayerStallUIManager : MonoSingleton<PlayerStallUIManager> {
        [SerializeField] private GameObject stallWindow;
        [SerializeField] private Transform stallCasesParent;
        [SerializeField] private StallCaseUI stallCasePrefab;
        [SerializeField] private GameObject buyStallCaseButton;

        [Header("StallCase Buy Window")]
        [SerializeField] private GameObject stallCaseBuyWindow;
        [SerializeField] private TMP_Text stallCaseBuyPrice;

        [Header("StallCase Window")]
        [SerializeField] private GameObject stallCaseWindow;
        [SerializeField] private StallCaseUI stallCaseStallCase;
        [SerializeField] private TMP_Text stallCaseItemQuantity;

        [Header("Sell Window")]
        [SerializeField] private GameObject sellWindow;
        [SerializeField] private Image sellItemIcon;
        [SerializeField] private TMP_Text sellItemQuantity;
        [SerializeField] private Button sellItemQuantityMinusButton;
        [SerializeField] private Button sellItemQuantityAddButton;
        [SerializeField] private TMP_Text sellItemPrice;
        [SerializeField] private Button sellItemPriceMinusButton;
        [SerializeField] private Button sellItemPriceAddButton;
        [SerializeField] private Button sellItemButton;

        [SerializeField] private InventoryUI inventory;

        // STALL WINDOW
        private PlayerStall currentStall;
        // STALL CASE WINDOW
        private StallCaseUI currentStallCase;
        // STALL CASE BUY WINDOW
        private int currentStallCasePrice;
        // SELL WINDOW
        private Item currentItem;
        private int currentQuantity;
        private int currentPrice;

        // GLOBAL
        private List<StallCaseUI> currentStallCases = new ();

        private void Awake() {
            SetInstance(this);
        }

        #region STALL
        public void ShowStallWindow(PlayerStall stall) {
            if (stall == null) {
                Debug.LogError("Try to show stall window with a null stall!");
                return;
            }
            currentStall = stall;
            UpdateStallCases();
            stallWindow.SetActive(true);
        }

        private void UpdateStallCases() {
            for (int index = 0; index < currentStall.GetStallCasesCount(); index++) {
                if (index >= currentStallCases.Count || currentStallCases[index] == null) {
                    currentStallCases.Add(Instantiate(stallCasePrefab, stallCasesParent));
                }
                StallCase stallCase = currentStall.GetStallCase(index);
                currentStallCases[index]?.ShowStallCase(stallCase, stallCase?.Item == null ? ShowSellWindow : ShowStallCaseWindow);
            }
            UpdateStallCaseBuyButton();
        }

        private void UpdateStallCaseBuyButton() {
            buyStallCaseButton.GetComponent<Button>().interactable = currentStall.GetStallCasePrice() <= InventoryManager.Get().GetMoney();
            buyStallCaseButton.SetActive(currentStall.GetStallCasesCount() < PlayerStall.MAX_STALL_CASE);
            buyStallCaseButton.transform.SetSiblingIndex(stallCasesParent.childCount);
        }

        public void HideStallWindow() {
            currentStall = null;

            stallWindow.SetActive(false);
        }
        #endregion

        #region STALL_CASE
        public void ShowStallCaseWindow(StallCaseUI stallCaseUI) {
            Debug.Log("TEST SELL WINDOW");
            currentStallCase = stallCaseUI;
            if (currentStallCase == null || currentStallCase.StallCase == null) {
                Debug.LogError($"Try to get stall case ui in player stall which doesn't exist!");
                return;
            }
            stallCaseStallCase.ShowStallCase(currentStallCase.StallCase, ShowStallCaseWindow);
            stallCaseItemQuantity.text = currentStallCase.StallCase.Quantity.ToString();

            stallCaseWindow.SetActive(true);
        }

        public void HideStallCaseWindow() {
            currentStallCase = null;

            stallCaseWindow.SetActive(false);
        }

        public void RemoveItemFromSell() {
            if (currentStallCase == null) {
                Debug.LogWarning("Try to remove null item from sell in player stall!");
                HideStallCaseWindow();
                return;
            }

            if (!currentStall.RemoveItemFromSell(currentStallCase.StallCase)) {
                Debug.LogError($"Try to remove stall case from player stall but failed!");
                HideStallWindow();
                return;
            }

            currentStallCase.ShowStallCase(null, ShowSellWindow);
            HideStallCaseWindow();
        }
        #endregion

        #region STALL_CASE_BUY
        public void ShowStallCaseBuyWindow() {
            stallCaseBuyPrice.text = $"Buy a new stall case to sell items for {currentStall.GetStallCasePrice()}$ ?";
            stallCaseBuyWindow.SetActive(true);
        }

        public void HideStallCaseBuyWindow() {
            stallCaseBuyWindow.SetActive(false);
        }

        public void BuyStallCase() {
            int stallCaseIndex = currentStall.BuyStallCase();
            if (stallCaseIndex < 0) {
                Debug.LogError("Try to buy a new stall case in player stall but failed!");
                HideStallCaseBuyWindow();
                return;
            }
            StallCaseUI stallCaseUI = Instantiate(stallCasePrefab, stallCasesParent);
            currentStallCases.Add(stallCaseUI);
            stallCaseUI.ShowStallCase(null, ShowSellWindow);
            UpdateStallCaseBuyButton();
            HideStallCaseBuyWindow();
        }
        #endregion

        #region SELL
        public void ShowSellWindow(StallCaseUI stallCaseUI) {
            currentStallCase = stallCaseUI;
            currentItem = null;
            currentQuantity = 0;
            currentPrice = 0;
            OnChangeItemSell();

            sellWindow.SetActive(true);
        }

        public void HideSellWindow() {
            currentStallCase = null;

            sellWindow.SetActive(false);
        }

        public void ShowInventoryWindow() {
            InventoryUIManager.Get().ShowInventory(inventory, ChangeItemSell, DataType.BaseData);
        }

        public void HideInventoryWindow() {
            InventoryUIManager.Get().HideInventory();
        }

        public void ChangeItemSell(Item item) {
            currentItem = item;
            OnChangeItemSell();
            HideInventoryWindow();
        }

        private void OnChangeItemSell() {
            if (currentItem == null) {
                sellItemIcon.sprite = DataManager.Get().UnknownIcon;
                currentQuantity = 0;
                currentPrice = 0;
            } else {
                sellItemIcon.sprite = currentItem.Data.GetIcon();
                currentQuantity = 1;
                currentPrice = currentItem.Data.Price;
            }
            sellItemButton.interactable = currentItem != null;
            OnChangeItemPrice();
            OnChangeItemQuantity();
        }

        public void ChangeItemQuantity(int modifier) {
            if (currentItem == null) {
                Debug.LogWarning("Try to change quantity of a null item in player stall sell window");
                return;
            }

            currentQuantity += modifier;
            OnChangeItemQuantity();
        }

        private void OnChangeItemQuantity() {
            sellItemQuantity.text = currentQuantity.ToString();
            int maxQuantity = (currentItem as QuantifiedItem)?.Quantity ?? 1;
            sellItemQuantityMinusButton.interactable = currentQuantity > 1;
            sellItemQuantityAddButton.interactable = currentQuantity <= maxQuantity && currentQuantity != 0;
        }

        public void ChangeItemPrice(int modifier) {
            if (currentItem == null) {
                Debug.LogWarning("Try to change price of a null item in player stall sell window");
                return;
            }

            currentPrice += modifier;
            OnChangeItemPrice();
        }

        private void OnChangeItemPrice() {
            sellItemPrice.text = currentPrice.ToString();
            sellItemPriceMinusButton.interactable = currentPrice > 1;
            sellItemPriceAddButton.interactable = currentPrice <= 999 && currentPrice != 0;
        }

        public void SellItem() {
            if (currentItem == null || currentQuantity <= 0 || currentPrice <= 0) {
                Debug.LogError($"Try to sell [{currentQuantity}] item [{currentItem?.Id}] at price [{currentPrice}] but can't do that!");
                HideSellWindow();
                return;
            }

            currentStallCase.StallCase.Item = currentItem;
            currentStallCase.StallCase.Quantity = currentQuantity;
            currentStallCase.StallCase.Price = currentPrice;
            currentStallCase.ShowStallCase(currentStallCase.StallCase, ShowStallCaseWindow);

            HideSellWindow();
        }
        #endregion
    }
}