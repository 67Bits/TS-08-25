using System;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Services/Inventory/Item Data List", fileName = "ItemDataList")]
public class ItemCatalog : ScriptableObject
{
    // Used to set up data in the Unity inspector
    [SerializeField] private List<ItemData> _dataList = new();
    // What really got us the data.
    [SerializeField] private Dictionary<ItemID, ItemData> _dataDict = new();

    public int Count()
    {
        if (_dataDict.Count == 0)
            SetUpDataStructures();

        return _dataDict.Count;
    }

    public ItemUIData GetUIData(ItemID type)
    {
        if (_dataDict.ContainsKey(type))
        {
            return _dataDict[type].UIData;
        }

        Debug.LogError("Missing Item UI data for " + type.ToString(), this);
        return null;
    }


    [Button]
    public void SetUpDataStructures()
    {
        // Get all types of itemsID
        var itemTypevalues = Enum.GetValues(typeof(ItemID));

        List<ItemData> oldDataPerType = _dataList;
        List<ItemData> newDataPerType = new(itemTypevalues.Length);

        foreach (var value in itemTypevalues)
        {
            ItemID itemType = (ItemID)value;
            ItemData itemGroup = null;

            // Rescuing items already in list to keep in the new one.
            if (oldDataPerType != null)
            {
                foreach (var oldItem in oldDataPerType)
                {
#if UNITY_EDITOR
                    // We set up by string so we can mach new enum orders.
                    if (oldItem.enumKey == itemType.ToString())
#else
                    // In Build, the enum order is planned to be matched already.
                    if (oldItem.Type == itemType)
#endif
                    {
                        itemGroup = oldItem;
                        break;
                    }
                }
            }

            // Adding new items for missing e items.
            if (itemGroup == null)
                itemGroup = new ItemData() {
#if UNITY_EDITOR
                    enumKey = itemType.ToString(),
#endif
                    Type = itemType,
                    UIData = new ItemUIData()
                };

            newDataPerType.Add(itemGroup);
        }


        _dataList = newDataPerType;
        _dataDict.Clear();

        foreach (var item in _dataList)
        {
            _dataDict.Add(item.Type, item);
        }

        // Reading in the list the items lost from last one, so we can check what happened.
        string missingMsg = " (Missing)";
        foreach (var oldItem in oldDataPerType)
        {
            bool found = false;

            if (_dataDict.ContainsKey(oldItem.Type)
                && (_dataDict[oldItem.Type].UIData == oldItem.UIData))
                continue;

            //if (!oldItem.UIData.Name.EndsWith(missingMsg))
            //    oldItem.UIData.SetSpecialName(oldItem.UIData.Name + missingMsg);

            _dataList.Add(oldItem);
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        var itemTypevalues = Enum.GetValues(typeof(ItemID));

        if (itemTypevalues.Length != _dataDict.Count)
        {
            SetUpDataStructures();
            return;
        }

        foreach (var value in itemTypevalues)
        {
            ItemID itemType = (ItemID)value;
            if (!_dataDict.ContainsKey(itemType))
            {
                SetUpDataStructures();
                break;
            }
        }
    }
#endif
}

[Serializable]
public class ItemData
{
#if UNITY_EDITOR
    [HideInInspector] public string enumKey;
#endif
    [@ReadOnly] public ItemID Type;
    public ItemUIData UIData;
}