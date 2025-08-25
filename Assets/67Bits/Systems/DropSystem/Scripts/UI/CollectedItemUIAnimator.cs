using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Inventory;

public class CollectedItemUIAnimator : Singleton<CollectedItemUIAnimator>
{

    [SerializeField] private InventoryListUI _inventoryListUI;

    [SerializeField] private Image _imagePrefab;
    private InventoryService _inventoryService;
    private InventoryListUI _inventoryScroll;
    private Amount<ItemID> _itemAmount;
    bool _wasCollected = false;

    private void AnimateItemCollection(Amount<ItemID> itemAmount, Vector3 initCamPosition)
    {
        _itemAmount = itemAmount;
        _wasCollected = false;
        if (_inventoryListUI.TryGetCounterPerItem(_itemAmount.Unit, out InventoryCounterUIElement itemCounter))
        {
            //TODO: Dot tween item going to UI.
        }
    }

    private void Start()
    {
        if (_inventoryListUI is null)
            Debug.LogError("ItemToInventoryListAnimation is missing InventoryListUI reference.", this);
        
        _inventoryService = InventoryService.Instance;

        if (_inventoryService is null)
            Debug.LogError("ItemToInventoryListAnimation is missing _inventoryService reference.", this);
    }

    private void OnDisable()
    {
        Collect();
    }

    private void OnDestroy()
    {
        Collect();
    }

    private void Collect()
    {
        if (_wasCollected)
            return;

        _wasCollected = true;

        _inventoryService.TryAddOnItemCount(_itemAmount.Unit, _itemAmount.Quantity);
    }
}
