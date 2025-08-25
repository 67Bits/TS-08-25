using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Inventoy;
using Inventoy.Settings;
using UnityEditor;

namespace Inventory
{
    public class InventoryService : IService, IDisposable
    {
        private InventoryData _data;
        private Dictionary<ItemID, int> ItemCount
        {
            get
            {
                if (_data == null) LoadData();
                return _data.ItemCount;
            }
        }

        private Dictionary<ItemID, Action<int>> CounterUpdateActionsDict
            = new Dictionary<ItemID, Action<int>>();

        #region Static references
        private static InventorySettings _settings;
        public static InventorySettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    InventorySettings[] savesSettings = Resources.LoadAll<InventorySettings>("");
                    if (savesSettings == null || savesSettings.Length < 1)
                        return null;
                    _settings = savesSettings[0];
                }
                return _settings;
            }
        }

        private static InventoryService _singletonInstance;
        public static InventoryService Instance
        {
            get
            {
                if (_singletonInstance == null)
                    Debug.LogError("Missing instance of InventoryService");
                return _singletonInstance;
            }
        }
        #endregion

        #region Initialization

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void OnApplicationInicialization()
        {
            if (Settings.AutoInitialize)
                (new InventoryService()).TryInitilializeSingleton();
        }

        public bool TryInitilializeSingleton()
        {
            if (_singletonInstance != null)
                return false;

            _singletonInstance = this;
            return true;
        }
        #endregion

        #region InventoryData
        /// <summary>
        /// Data that is saved and loaded with the inventory System.
        /// </summary>
        private class InventoryData
        {
            public Dictionary<ItemID, int> ItemCount = new Dictionary<ItemID, int>();
        }
        #endregion

        #region ItemData
        public ItemUIData GetItemUIData(ItemID itemID)
        {
            return Settings.Catalog.GetUIData(itemID);
        }
        #endregion

        #region Item count
        public int GetItemCount(ItemID item)
        {
            if (ItemCount.ContainsKey(item))
                return ItemCount[item];

            return 0;
        }

        public bool TrySetItemCount(ItemID item, int quantity)
        {
            if (quantity < 0) // Can'T accept negative items.
                return false;

            SetItemQuantity(item, quantity);

            return true;
        }

        public bool TryAddOnItemCount(ItemID item, int quantity)
        {
            int currentCount = GetItemCount(item);
            currentCount += quantity;
            return TrySetItemCount(item, currentCount);
        }

        [QFSW.QC.Command("Set-Item-Quantity")]
        [QFSW.QC.CommandDescription("Add value in Inventory item quantity.")]
        private void SetItemQuantity(ItemID item, int quantity)
        {
            if (ItemCount.ContainsKey(item))
                ItemCount[item] = quantity;
            else
                ItemCount.Add(item, quantity);

            CallCountUpdateAction(item);
        }

        #endregion

        #region Item count Action
        /// <summary>
        /// Invoke the action for a specific item, if there is an initialized action.
        /// Actions receive the item counter as his int parameter.
        /// </summary>
        /// <param name="item">The key item for the action.</param>
        private void CallCountUpdateAction(ItemID item)
        {
            if (CounterUpdateActionsDict.ContainsKey(item))
                CounterUpdateActionsDict[item]?.Invoke(GetItemCount(item));
        }

        /// <summary>
        /// Subscribe a method to action called when item count is updated.
        /// </summary>
        /// <param name="item">The item counted</param>
        /// <param name="action">Method subscribed.</param>
        public void SubscribeToCountUpdateAction(ItemID item, Action<int> action)
        {
            if (!CounterUpdateActionsDict.ContainsKey(item))
            {
                Action<int> newAction = (itemCounterValue) => { };
                CounterUpdateActionsDict.Add(item, newAction);
            }

            CounterUpdateActionsDict[item] += action;
        }

        /// <summary>
        /// Unsubscribe a method from action called when item count is updated.
        /// </summary>
        /// <param name="item">The item counted</param>
        /// <param name="action">Method unsubscribed.</param>
        public void UnsubscribeFromCountUpdateAction(ItemID item, Action<int> action)
        {
            if (!CounterUpdateActionsDict.ContainsKey(item))
                return;

            CounterUpdateActionsDict[item] -= action;
        }
        #endregion

        #region Save and Load
        private void SaveData()
        {
            SaveSystem.SaveDataFile(_data);
        }

        private void LoadData()
        {
            if (!SaveSystem.TryLoadDataFile(out _data))
            {
                _data = new InventoryData();
            }

            SaveSystem.OnSave -= SaveData;
            SaveSystem.OnSave += SaveData;
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Inventory System/Reset Items Count")]
#endif
        [QFSW.QC.CommandDescription("Reset data of Inventory. All item quantities go to zero.")]
        public static void ResetDataSaved()
        {
            SaveSystem.SaveDataFile(new InventoryData());
        }
        #endregion

        public void Dispose()
        {
            SaveSystem.OnSave -= SaveData;
        }
    }

}

namespace Inventory.Test
{
    public class InventoryServiceTest
    {

        [QFSW.QC.Command("Set-Item-Quantity")]
        [QFSW.QC.CommandDescription("Add value in Inventory item quantity.")]
        public static void SetItemQuantity(ItemID item, int quantity)
        {
            InventoryService.Instance.TrySetItemCount(item, quantity);
        }
    }
}