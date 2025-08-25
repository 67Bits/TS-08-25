using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [Flags]
    public enum ItemCarrierType
    {
        None = 0,         // Represents no selection
        Player = 1 << 0,  // 1
        Store = 1 << 1,   // 2
        Deposit = 1 << 2, // 4
        Ally = 1 << 3, // 8
        Enemy = 1 << 4 // 16
    }
}