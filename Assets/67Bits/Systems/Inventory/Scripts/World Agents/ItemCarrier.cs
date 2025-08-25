using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;

/// <summary>
/// An agent in game world that can hold items.
/// A mine, a collector minion or a deposit.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ItemCarrier : MonoBehaviour, IItemCarrier
{
    [SerializeField] private ItemCarrierType _type;
    public ItemCarrierType Type { get => GetType(); }

    [SerializeField] private List<Amount<ItemID>> _CarriedItemsCapacity = new();
    [SerializeField] private List<Amount<ItemID>> _carriedItems = new();

    public ItemCarrierType GetType()
    {
        return Type;
    }

    #region ListHelper
    private int GetAmountInList(ItemID item, List<Amount<ItemID>> list)
    {
        foreach (var amount in list)
        {
            if (amount.Unit == item)
                return amount.Quantity;
        }

        return 0;
    }

    private void SetAmountInList(Amount<ItemID> newAmount, List<Amount<ItemID>> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Unit == newAmount.Unit)
            {
                list[i] = newAmount;
                return;
            }
        }

        // case no entry in list has same itemID
        list.Add(newAmount);
    }
    #endregion

    #region Capacity
    public int RemainingItemCapacity(ItemID item)
    {
        int quantityLimit = GetAmountInList(item, _CarriedItemsCapacity);

        int quantityCurrent = GetAmountInList(item, _carriedItems);

        if (quantityCurrent > quantityLimit)
            return 0;

        return quantityLimit - quantityCurrent;
    }
    #endregion

    #region Carry Amount
    private void Carry(Amount<ItemID> amount)
    {
        Amount<ItemID> newAmount = amount + GetAmountInList(amount.Unit, _carriedItems);
    }

    private void Withdraw(Amount<ItemID> amount)
    {
        int newQuantity = GetAmountInList(amount.Unit, _carriedItems) - amount.Quantity;
        Amount<ItemID> newAmount = new Amount<ItemID>(amount.Unit, newQuantity);
        SetAmountInList(newAmount, _carriedItems);
    }

    public bool CanCarry(Amount<ItemID> amount)
    {
        return RemainingItemCapacity(amount.Unit) <= amount.Quantity;
    }

    public bool CanWithdraw(Amount<ItemID> amount)
    {
        return GetAmountInList(amount.Unit, _CarriedItemsCapacity) >= amount.Quantity;
    }

    public bool TryCarry(Amount<ItemID> amount)
    {
        if (CanCarry(amount))
            return false;

        Carry(amount);

        return true;
    }

    public bool TryWithdraw(Amount<ItemID> amount)
    {
        if (CanWithdraw(amount))
            return false;

        Carry(amount);

        return true;
    }
    #endregion
}
