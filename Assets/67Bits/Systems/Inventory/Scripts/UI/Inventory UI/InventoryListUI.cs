using Inventory;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using HideableElement;

/// <summary>
/// A list of items in UI.
/// </summary>
public class InventoryListUI : HideableElementsListUI
{
    [Header("InventoryListUI")]
    [SerializeField] private InventoryCounterUIElement _itemCounterUIPrefab;

    [Space]
    [SerializeField] private List<InventoryCounterUIElement> _countersList = new();

    [Space]
    [ReadOnly] [SerializeField] private ItemCatalog _itemCatalog;

    private Dictionary<ItemID, InventoryCounterUIElement> _counterPerItem = new Dictionary<ItemID, InventoryCounterUIElement>();
    private int ItemsVisible;
    private InventoryService _inventoryService;

    #region Monobehavior

    private void OnDisable()
    {
        foreach (var counter in _countersList)
        {
            CollectingUI.Instance.UnsubscribeFromCollectors(counter.ItemID, counter);
        }
    }

    private void Start()
    {
        _inventoryService = InventoryService.Instance;
        UpdateItemsLists();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_elementsParent);
    }

    private void OnDestroy()
    {
        DisposeAllCounters();
    }
    #endregion

    private void UpdateItemsLists()
    {
        SetCountersListToSize(_itemCatalog.Count());
        _counterPerItem.Clear();

        for (int i = 0; i < _itemCatalog.Count(); i++)
        {
            ItemID item = (ItemID)i;
            InventoryCounterUIElement newItemCounter = _countersList[i];

            _counterPerItem.Add(item, newItemCounter);

            ItemUIData UIData = _itemCatalog.GetUIData(item);
            newItemCounter.SetUpUIData(item);

            if (newItemCounter.TryGetHideableUIElement(out HideableUIElement hideable))
            {
                AddHideableElement(hideable);

                if (UIData.IsInitiallyVisible)
                    SetItemVisibility(item, Visibility.AlwaysVisible);
                else
                    SetItemVisibility(item, Visibility.Hideable);
            }
            else
                Debug.Log("Missing _hideableUIElement.\n\n", newItemCounter);

            CollectingUI.Instance.SubscribeToCollectors(newItemCounter.ItemID, newItemCounter);
        }

        ShuffleListPerVisibility();
    }

    #region Item visibility
    private void SetItemVisibility(ItemID item, Visibility visibility)
    {
        if (_counterPerItem.ContainsKey(item))
        {
            _counterPerItem[item].GetComponent<HideableUIElement>().SetVisibility(visibility);
            _counterPerItem[item].UpdateVisual();
        }
    }
    #endregion

    public void SetCountersListToSize(int idealSize)
    {
        int listCount = _countersList.Count;

        if (listCount > idealSize)
        {
            for (int i = listCount - 1; i > idealSize - 1; i--)
            {
                var go = _countersList[i];
                _countersList.RemoveAt(i);
                DisposeCounterInstance(go);
            }
        }
        else if (listCount < idealSize)
        {
            for (int i = 0; i < idealSize - listCount; i++)
                _countersList.Add(GetNewCounterInstance());
        }
    }

    public InventoryCounterUIElement GetNewCounterInstance()
    {
        GameObject newItemGO = Instantiate(_itemCounterUIPrefab.gameObject);
        newItemGO.transform.parent = _elementsParent;
        return newItemGO.GetComponent<InventoryCounterUIElement>();
    }

    public bool TryGetCounterPerItem(ItemID item, out InventoryCounterUIElement itemCounter)
    {
        if (_counterPerItem.ContainsKey(item))
        {
            itemCounter = _counterPerItem[item];
            return true;
        }
        else
        {
            itemCounter = null;
            return false;
        }
    }

    public void DisposeCounterInstance(InventoryCounterUIElement counter)
    {
        GameObject itemGO = counter.gameObject;
        Destroy(itemGO);
    }

    public void DisposeAllCounters()
    {
        foreach (var counter in _countersList)
            DisposeCounterInstance(counter);
        
        _countersList.Clear();
        _counterPerItem.Clear();
    }
}
