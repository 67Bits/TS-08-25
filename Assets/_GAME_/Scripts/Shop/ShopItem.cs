using System;
using System.Collections.Generic;
using UnityEngine;
using Turret;

namespace TGR
{
    [CreateAssetMenu(fileName = "ShopItem", menuName = "Scriptable Objects/ShopItem")]
    public abstract class ShopItem : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Icon;
        public int Price;
        public abstract void Apply();

        public abstract void Reset();
    }
}
