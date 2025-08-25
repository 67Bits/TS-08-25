using System.Collections.Generic;
using UnityEngine;
using static Talents.PermanentUpgradeApplier;

namespace Talents
{
    public abstract class UpgradeBase : ScriptableObject
    {
        public UpgradeType upgradeType = UpgradeType.InLevel;
        public TierType rarity = TierType.Rare;
        public string upgradeName = "";
        [TextArea] public string upgradeDescription = "";
        public Sprite upgradeImage = null;
        public List<UpgradeBase> chainedUpgrades = new List<UpgradeBase>();
        public int stackableCount = 1;

        public abstract void ApplyUpgrade(Transform t);

        public abstract void RemoveUpgrade(Transform t);

        public enum UpgradeType
        {
            InLevel,
            Permanent
        }
    }
}