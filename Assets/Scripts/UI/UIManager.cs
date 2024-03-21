using System;
using System.Collections;
using System.Collections.Generic;
using Agricultura.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Agricultura {
    public class UIManager : MonoSingleton<UIManager> {
        [Header("Money Parameters")]
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text gemsText;

        [Header("DownBar Parameters")]
        [SerializeField] private Image bagItemImage;
        [SerializeField] private InventoryUI bagInventory;
        [SerializeField] private GameObject selectedBagItem;
        [SerializeField] private GameObject selectedScythe;
        [SerializeField] private GameObject selectedHand;

        [Header("Pause Window")]
        [SerializeField] private GameObject pauseWindow;
        [SerializeField] private GameObject confirmResetWindow;
        [SerializeField] private GameObject privacyPolicyWindow;

        private RectTransform currentWindow;
        private Coroutine moveCoroutine;

        private void Awake() {
            SetInstance(this);
        }

        private void Start() {
            UpdateMoney(InventoryManager.Get().GetMoney());
            UpdateGems(InventoryManager.Get().GetGems());
            InventoryManager.Get().OnMoneyChange += UpdateMoney;
            InventoryManager.Get().OnGemsChange += UpdateGems;
        }

        public void Reset() {
            pauseWindow.SetActive(false);
            privacyPolicyWindow.SetActive(false);
            confirmResetWindow.SetActive(false);
        }

        #region DOWNBAR
        public void OnClickBag() {
            InventoryUIManager.Get().ShowInventory(bagInventory, OnClickItem, DataType.BaseData);
        }

        private void OnClickItem(Item item) {
            bagItemImage.sprite = item.Data.GetIcon();
            bagItemImage.gameObject.SetActive(true);

            InventoryManager.Get().SetBagItem(item);
            OnChangeItem(selectedBagItem);
            OnCloseBag();
        }

        public void OnCloseBag() {
            InventoryUIManager.Get().HideInventory();
        }

        public void OnClickBagItem() {
            InventoryManager.Get().SelectBagItem();
            OnChangeItem(selectedBagItem);
        }

        public void OnClickHand() {
            InventoryManager.Get().SelectHand();
            OnChangeItem(selectedHand);
        }

        public void OnClickScythe() {
            InventoryManager.Get().SelectScythe();
            OnChangeItem(selectedScythe);
        }

        public void OnClickBook() {
            BookUIManager.Get().ShowBook(null, BookUIManager.BookCategoryType.PLANT);
        }

        public void OnEmptyHand() {
            bagItemImage.gameObject.SetActive(false);
            OnChangeItem(selectedHand);
        }

        private void OnChangeItem(GameObject selectedItem) {
            selectedScythe.SetActive(selectedScythe == selectedItem);
            selectedHand.SetActive(selectedHand == selectedItem);
            selectedBagItem.SetActive(selectedBagItem == selectedItem);
        }
        #endregion

        #region MENU
        public void OnClickPause() {
            pauseWindow.SetActive(true);
        }

        public void OnClickAchievements() {
            AchievementUIManager.Get().ShowAchievements(AchievementsManager.Get().Achievements);
        }

        public void OnClickSave() {
            GameManager.Get().Save();
        }

        public void OnClickQuit() {
            GameManager.Get().Save();
            Application.Quit();
        }

        public void OnClickReset() {
            confirmResetWindow.SetActive(true);
        }

        public void OnClickConfirmReset() {
            GameManager.Get().Reset();
        }

        public void OnClickConfirmResetClose() {
            confirmResetWindow.SetActive(false);
        }

        public void OnClickPrivacyPolicy() {
            privacyPolicyWindow.SetActive(true);
        }

        public void OnClickPrivacyPolicyClose() {
            privacyPolicyWindow.SetActive(false);
        }

        public void OnClickPauseClose() {
            pauseWindow.SetActive(false);
        }
        #endregion

        private void UpdateMoney(int money) {
            moneyText.text = money.ToString();
        }

        private void UpdateGems(int gems) {
            gemsText.text = gems.ToString();
        }
    }
}