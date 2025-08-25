using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using System;

/// <summary>
/// Represents the inventory existing in UI as in the game world, on the player character.
/// </summary>
public class PlayerInventory : MonoBehaviour, IItemCarrier
{
    [SerializeField] private ItemCarrierType _itemCarrierType = ItemCarrierType.Player;

    public ItemCarrierType GetType()
    {
        return _itemCarrierType;
    }

    #region Carry Amount
    public bool CanCarry(Amount<ItemID> amount)
    {
        return true;
    }

    public bool CanWithdraw(Amount<ItemID> amount)
    {
        int itemCount = InventoryService.Instance.GetItemCount(amount.Unit);
        return itemCount >= amount.Quantity;
    }

    public bool TryCarry(Amount<ItemID> amount)
    {
        return InventoryService.Instance.TryAddOnItemCount(amount.Unit, amount.Quantity);
    }

    public bool TryWithdraw(Amount<ItemID> amount)
    {
        if (!CanWithdraw(amount)) return false;

        return InventoryService.Instance.TryAddOnItemCount(amount.Unit, -amount.Quantity);
    }
    #endregion
}