using DG.Tweening;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectedItemIcon : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private Image _itemImage;
    private Amount<ItemID> _itemAmount;
    private InventoryCounterUIElement _itemCounter;

    [Header("Configurations")]
    [Tooltip("Duration of animation")]
    [SerializeField] private float _duration = 0.5f; 

    void Init(Amount<ItemID> itemAmount, InventoryCounterUIElement itemCounter)
    {
        _itemAmount = itemAmount;
        _itemCounter = itemCounter;
        _itemImage.sprite = InventoryService.Instance.GetItemUIData(itemAmount.Unit).Sprite;
    }

    void StartAnimation()
    {
        this.gameObject.SetActive(true);
        this.transform.DOMove((_itemCounter.transform as RectTransform).position, _duration);
    }

    void FinishAnimation()
    {

    }
}
