using Modifiables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrades
{
    public interface IUpgrade
    {
        public abstract void ApplyUpdate(IUpgradeTarget character);
        public abstract void ClearUpdate(IUpgradeTarget character);
    }
}
