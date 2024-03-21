using System;
using Agricultura.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class StallUIManager : MonoSingleton<StallUIManager> {
        [SerializeField] private GameObject stallWindow;
        [SerializeField] private GameObject meatStallImage;
        [SerializeField] private GameObject dairyStallImage;
        [SerializeField] private GameObject groceryStallImage;
        [SerializeField] private StallCaseUI[] stallCases;
        [SerializeField] private Button previousRowButton;
        [SerializeField] private Button nextRowButton;
        [SerializeField] private TMP_Text rowNumber;

        [SerializeField] private GameObject stallCaseWindow;
        [SerializeField] private StallCaseUI stallCaseStallCase;
        [SerializeField] private TMP_Text stallCaseItemQuantity;
        [SerializeField] private TMP_Text stallCaseItemTotalCost;
        [SerializeField] private Button stallCaseBuyButton;
        [SerializeField] private Button stallCaseMinusQuantityButton;
        [SerializeField] private Button stallCaseAddQuantityButton;

        private Stall currentStall;
        private StallCaseUI currentStallCase;
        private int currentRow;
        private BaseData currentItem;
        private int currentQuantity;

        private void Awake() {
            SetInstance(this);
        }

        public void ShowStall(Stall stall) {
            if (stall == null) {
                Debug.LogError("Try to show stall window with a null stall!");
                return;
            }
            currentStall = stall;
            currentRow = 1;
            OnChangeRow();
            UpdateStallCases();

            //TODO Switch image depending on stall
            stallWindow.SetActive(true);
        }

        public void HideStall() {
            currentStall = null;

            stallWindow.SetActive(false);
        }

        public void ShowNextStallCasesRow() {
            if (currentStall == null) {
                Debug.LogError("Try to change row in stall window with a null stall!");
                return;
            }
            currentRow++;
            OnChangeRow();
            UpdateStallCases();
        }

        public void ShowPreviousStallCasesRow() {
            if (currentStall == null) {
                Debug.LogError("Try to change row in stall window with a null stall!");
                return;
            }

            currentRow--;
            OnChangeRow();
            UpdateStallCases();
        }

        private void UpdateStallCases() {
            int stallCaseCount = currentStall.GetStallCasesCount();
            for (int index = 0; index < stallCases.Length; index++) {
                stallCases[index]?.ShowStallCase(index >= stallCaseCount ? null : currentStall.GetStallCase((currentRow - 1) * stallCases.Length + index), index >= stallCaseCount ? null : ShowStallCase);
            }
        }

        private void OnChangeRow() {
            int maxRow = Mathf.CeilToInt(currentStall.GetStallCasesCount() / (float)stallCases.Length);

            previousRowButton.interactable = currentRow > 1;
            nextRowButton.interactable = currentRow <  maxRow;
            rowNumber.text = maxRow <= 0 ? "" : $"{currentRow}/{maxRow}";
        }

        public void ShowStallCase(StallCaseUI stallCaseUI) {
            if (stallCaseUI == null) {
                Debug.LogError($"Show stallCase ui in stall [{currentStall?.GetStallId()}] which doesn't exist!");
                return;
            }
            currentStallCase = stallCaseUI;
            currentItem = currentStallCase.StallCase?.Item.Data;
            if (currentItem == null) {
                Debug.LogError($"Try to show stall case with item [{currentStallCase.StallCase?.Item.Id}] in stall [{currentStall?.GetStallId()}] which doesn't exist!");
                return;
            }

            currentQuantity = 1;
            OnChangeItemQuantity();

            stallCaseStallCase.ShowStallCase(currentStallCase.StallCase, ShowStallCase);
            stallCaseWindow.SetActive(true);
        }

        public void HideStallCase() {
            currentItem = null;
            currentStallCase = null;
            stallCaseWindow.SetActive(false);
        }

        public void ChangeBuyQuantity(int modifier) {
            currentQuantity += modifier;
            OnChangeItemQuantity();
        }

        public void BuyItem() {
            if (currentStall == null || currentStallCase == null || currentItem == null || currentQuantity <= 0) {
                Debug.LogError($"Try to buy [{currentQuantity}] item [{currentItem?.Id}] in stall [{currentStall?.GetStallId()}] but couldn't!");
                HideStallCase();
                return;
            }

            currentStall.BuyItem(currentItem.Id, currentQuantity);
            currentStallCase.ShowStallCase(currentStallCase.StallCase, ShowStallCase);
            HideStallCase();
        }

        private void OnChangeItemQuantity() {
            if (currentStall == null || currentItem == null) {
                Debug.LogWarning($"Try to update item quantity [{currentItem?.Id}] in stall [{currentStall?.GetStallId()}] but couldn't!");
                return;
            }
            stallCaseItemQuantity.text = currentQuantity.ToString();
            int cost = currentStallCase.StallCase.Price * currentQuantity;
            stallCaseItemTotalCost.text = cost.ToString();
            stallCaseBuyButton.interactable = cost <= InventoryManager.Get().GetMoney();

            stallCaseMinusQuantityButton.interactable = currentQuantity > 1;
            stallCaseAddQuantityButton.interactable = currentQuantity < currentStallCase.StallCase.Quantity;
        }
    }
}