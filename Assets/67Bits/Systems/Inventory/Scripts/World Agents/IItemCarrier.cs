
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public interface IItemCarrier
    {
        public ItemCarrierType GetType();
        public bool CanCarry(Amount<ItemID> amount);
        public bool CanWithdraw(Amount<ItemID> amount);
        public bool TryCarry(Amount<ItemID> amount);
        public bool TryWithdraw(Amount<ItemID> amount);
    }
}
