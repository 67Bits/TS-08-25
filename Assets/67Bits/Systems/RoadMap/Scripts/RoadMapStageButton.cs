using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Upgrades;
using MoreMountains.Feedbacks;
using Inventory;

namespace SSBRoadmap
{
    public class RoadMapStageButton : RoadMapElement
    {
        [SerializeField] private Button _button;
        [Header("Visuals")]

        [Header("Price")]
        [SerializeField] private Image _currencyIcon;
        [SerializeField] private TextMeshProUGUI _priceText;

        [Header("Item")]
        [SerializeField] private Image _rewardItemIcon;
        [SerializeField] private TextMeshProUGUI _rewardText;

        [Header("In Game Feels")]
        [SerializeField] protected MMF_Player _onNextFeel;
        [SerializeField] protected MMF_Player _onBuyFeel;
        [SerializeField] protected MMF_Player _onAvailableFeel;
        [SerializeField] protected MMF_Player _onBlockedByMoneyFeel;

        private UpgradeData _upgradeData;

        public UpgradeData UpgradeData
        {
            get => _upgradeData;
            set
            {
                _upgradeData = value;
                UpdateButtonValues();
            }
        }
        public Button Button => _button;

        protected virtual void UpdateButtonValues()
        {
            _currencyIcon.sprite = GetCurrencyIcon();
            _priceText.text = GetPriceText();
            _rewardItemIcon.sprite = GetRewardItemIcon();
            _rewardText.text = GetRewardText();
        }

        protected virtual Sprite GetCurrencyIcon()
        {
            return InventoryService.Instance.GetItemUIData(UpgradeData.Price.Unit).Sprite;
        }
        protected virtual Sprite GetRewardItemIcon()
        {
            return _upgradeData.Sprite;
        }
        protected virtual string GetPriceText()
        {
            return _upgradeData.Price.Quantity.ToString();
        }
        protected virtual string GetRewardText()
        {
            return _upgradeData.ShortValueDescription;
        }

        public override void SetLocked(bool loading = false)
        {
            _button.interactable = false;
            base.SetLocked(loading);
        }

        public override void SetNext(bool loading = false)
        {
            _button.interactable = false;
            base.SetNext(loading);
            if (!loading)
                _onNextFeel?.PlayFeedbacks();
        }
        public override void SetAvailable(bool loading = false)
        {
            _button.interactable = true;
            base.SetAvailable(loading);
            if (!loading)
                _onAvailableFeel?.PlayFeedbacks();
        }

        public override void SetBought(bool loading = false)
        {
            _button.interactable = false;
            base.SetBought(loading);
            if (!loading)
                _onBuyFeel?.PlayFeedbacks();
        }

        public void SetNotEnoughMoney(bool loading = false)
        {
            _button.interactable = false;
            if (!loading)
                _onBlockedByMoneyFeel?.PlayFeedbacks();
        }

    }
}