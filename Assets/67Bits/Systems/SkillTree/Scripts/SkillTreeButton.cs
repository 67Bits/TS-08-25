using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Upgrades;
using Inventory;
using MoreMountains.Feedbacks;

public class SkillTreeButton : MonoBehaviour
{
    [SerializeField] private Image _upgradeImage;
    [SerializeField] private Image _priceItemImage;
    [SerializeField] private TextMeshProUGUI _upgradeBonusText;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private Color _hasPurchaseAmountCollor = Color.white;
    [SerializeField] private Color _dontHasPurchaseAmountCollor = Color.red;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _line;

    [ReadOnly] public UpgradeData upgradeData;
    public Action<SkillTreeButton> OnUpgradePurchase = (STButton) => { };
    public InventoryService InventoryService;
    public bool WasPurchased = false;
    public ButtonState? lastButtonState;

    [SerializeField] private MMF_Player OnAvailableFeel;
    [SerializeField] private MMF_Player OnBlockedFeel;
    [SerializeField] private MMF_Player OnBuyFeel;
    [SerializeField] private MMF_Player OnBoughtLoad;

    private void Start()
    {
        _upgradeImage.sprite = upgradeData.Sprite;
        _upgradeBonusText.text = upgradeData.ShortValueDescription;
        _priceItemImage.sprite = InventoryService.Instance.GetItemUIData(upgradeData.Price.Unit).Sprite;
        _priceText.text = upgradeData.Price.Quantity.ToString();
    }

    public void SetLineEnabled(bool state) => _line.SetActive(state);

    public void UpdatePriceTextColor()
    {
        if (HasItemAmountToPurchase())
            _priceText.color = _hasPurchaseAmountCollor;
        else
            _priceText.color = _dontHasPurchaseAmountCollor;
    }

    public void UpdateUI(bool EnableLine, ButtonState state)
    {
        SetLineEnabled(EnableLine);
        UpdatePriceTextColor();
        SetButtonState(state);
    }

    public bool HasItemAmountToPurchase()
    {
        var amountInInventory = InventoryService.GetItemCount(upgradeData.Price.Unit);
        return (amountInInventory >= upgradeData.Price.Quantity);
    }

    public void OnClick()
    {
        Debug.Log("On button click", this.gameObject);
        OnUpgradePurchase?.Invoke(this);
    }

    public void SetButtonInteractable(bool value)
    {
        _button.interactable = value;
    }

    public void SetButtonState(ButtonState state)
    {
        if ((lastButtonState != null) && lastButtonState == state)
            return;

        lastButtonState = state;

        switch (state)
        {
            case ButtonState.Blocked:
                OnBlockedFeel.PlayFeedbacks();
                break;

            case ButtonState.Available:
                OnAvailableFeel.PlayFeedbacks();
                break;

            case ButtonState.Buying:
                OnBuyFeel.PlayFeedbacks();
                break;

            case ButtonState.Bought:
                OnBoughtLoad.PlayFeedbacks();
                break;

            default:
                throw new NotImplementedException($"State {state} was not implemented");
        }
    }

    public enum ButtonState
    {
        Blocked,
        Available,
        Buying,
        Bought,
    }
}
