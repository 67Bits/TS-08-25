using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Inventory;
using System.Collections.Generic;
using System;
using HideableElement;

/// <summary>
/// A counter of one specific item in UI.
/// Usually, part of an InventoryListUI.
/// </summary>
public class InventoryCounterUIElement : MonoBehaviour, ICollectorUIElement
{
    [ReadOnly] [SerializeField] private ItemID _itemID;

    [SerializeField] private TMP_Text _labelText;
    [SerializeField] private CanvasGroup _alphaCG;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private List<Image> _tonedImagesList;
    [SerializeField] private float _updatevelocity = 11.0f;

    [Header("Color when zero")]
    [SerializeField] private bool _KeepVisibleIfNotZero = false;
    [SerializeField] private Color _zeroColor = new Color(1, 1, 1, 0.5f);

    private ItemUIData _itemUIData;
    private HideableUIElement _hideableUIElement;
    private List<Amount<ItemID>> _lockedAmounts = new();
    private float _initialAlpha = -1;
    private float _currentQuantity = -1;

    public ItemID ItemID => _itemID;
    public bool IsUIDataSetUp => _itemUIData != null;
    public bool HasHideableUIElementSet => (_hideableUIElement != null);


    #region Visuals update
    public void UpdateVisual()
    {
        if (!HasHideableUIElementSet)
        {
            if (!TryGetHideableUIElement(out _hideableUIElement))
                return;
        }

        if (_KeepVisibleIfNotZero && (InventoryService.Instance.GetItemCount(_itemID) > 0))
            _hideableUIElement.SetAsTemporarilyUnhideable();

        if (_hideableUIElement.Visibility == Visibility.AlwaysVisible)
            UpdateBackgroundAlpha(1.0f);
        else
            UpdateBackgroundAlpha(0.7f);

        void UpdateBackgroundAlpha(float alphaValue)
        {
            if (!_alphaCG)
                return;

            if (_initialAlpha == -1)
                _initialAlpha = _alphaCG.alpha;

            alphaValue = Mathf.Clamp(alphaValue, 0.0f, 1.0f);

            _alphaCG.alpha = alphaValue;
        }
    }
    #endregion

    #region Initialization
    public void SetUpUIData(ItemID id)
    {
        _itemID = id;
        _itemUIData = InventoryService.Instance.GetItemUIData(_itemID);
        _iconImage.sprite = _itemUIData.Sprite;

        foreach (Image image in _tonedImagesList)
            image.color = _itemUIData.RelatedColor;
    }

    public bool TryGetHideableUIElement(out HideableUIElement hideable)
    {
        if (this.gameObject.TryGetComponent(out hideable))
        {
            _hideableUIElement = hideable;
            return true;
        }
        return false;
    }
    #endregion

    #region Update counter Quantity
    public void PlayCounterUpdateAnimation()
    {
        int holdQuantity = 0;

        // Locked amounts are hold for animation purposes.
        foreach (Amount<ItemID> amount in _lockedAmounts)
            holdQuantity += amount.Quantity;

        // When animations are happening, we might need to exhib another value in UI.
        int aimedQuantity = InventoryService.Instance.GetItemCount(_itemID) - holdQuantity;

        if (_currentQuantity != aimedQuantity)
        {
            // If value is moving up (was gained) or down (was spent)
            float updateDirection = (_currentQuantity > aimedQuantity) ? -1.0f : 1.0f;

            float diff = (aimedQuantity - _currentQuantity) * updateDirection;

            // If the value is tooo much, we go faster.
            float extraSpeed = (diff > 99) ? 90 : (diff > 9) ? 9 : 1;
            float frameUpdatedQuantiy = _updatevelocity * updateDirection * extraSpeed * Time.deltaTime;

            _currentQuantity += frameUpdatedQuantiy;

            if (((updateDirection == 1) && (_currentQuantity > aimedQuantity))
                || ((updateDirection == -1) && (_currentQuantity < aimedQuantity)))
                _currentQuantity = aimedQuantity;

            UpdateCounter((int)_currentQuantity);
        }
    }

    private void UpdateCounter(int counterQuantity)
    {
        _labelText.text = counterQuantity.ToString();
        UpdateVisual();
    }
    #endregion

    #region Hold counter Quantity Animation
    /// <summary>
    /// Locked amounts don't appear in the counter till be released. Used in animations.
    /// </summary>
    private Amount<ItemID> LockAmount(in int quantity)
    {
        Amount<ItemID> amount = new Amount<ItemID>(_itemID, quantity);
        _lockedAmounts.Add(amount);
        return amount;
    }

    private void ReleaseAmount(in Amount<ItemID> amount)
    {
        if (_lockedAmounts.Contains(amount))
            _lockedAmounts.Remove(amount);
    }

    List<Coroutine> coroutines;

    public void HoldQuantityForAnimation(int quantity, float waitTime)
    {
        CoroutineRunner.Instance.StartCoroutine(HoldQuantityForAnimationRoutine(quantity, waitTime));
    }

    private IEnumerator HoldQuantityForAnimationRoutine(int quantity, float waitTime)
    {
        Amount<ItemID> amount = LockAmount(quantity);
        yield return new WaitForSeconds(waitTime);
        if (this == null) yield break; // since it will run in the runner.
        ReleaseAmount(amount);
    }
    #endregion

    #region Collecting Item
    public TrackedPosition CollectingUIPosition()
    {
        return new TrackedPosition(this.transform);
    }

    public void OnCollectAnimationStart()
    {
        _hideableUIElement.SetAsTemporarilyUnhideable();
        _hideableUIElement.Show();
    }

    public void OnCollect()
    {
        _hideableUIElement.SetAsTemporarilyUnhideable();
        _hideableUIElement.Show();
    }
    #endregion

    #region Monobehavior events
    private void OnEnable()
    {
        CollectingUI.Instance.SubscribeToCollectors(_itemID, this);
    }

    private void OnDisable()
    {
        CollectingUI.Instance.UnsubscribeFromCollectors(_itemID, this);
        _lockedAmounts.Clear();
    }

    private void Start()
    {
        if (!IsUIDataSetUp)
        {
            Debug.LogError("ItemCounterUI missing item.", this.gameObject);
            this.gameObject.SetActive(false);
            return;
        }

        _currentQuantity = InventoryService.Instance.GetItemCount(_itemID);
        UpdateCounter((int)_currentQuantity);
    }

    private void LateUpdate()
    {
        PlayCounterUpdateAnimation();
    }
    #endregion
}
