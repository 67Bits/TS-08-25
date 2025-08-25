//using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Modifiables;
using Inventory;

namespace Upgrades
{
    [System.Serializable]
    //[CreateAssetMenu(fileName = "new IUpgrade", menuName = "Scriptable Objects/Upgrades/UpgradeData/IUpgrade")]
    public abstract class UpgradeData : ScriptableObject
    {
        // Name, Description and ShortValueDescription uses depend on UI use case.
        public string Name;
        public string Description;
        public string ShortValueDescription;
        public Sprite Sprite;
        public Amount<ItemID> Price;
        public SkillTreeUpgradeType SkillTreeUpgradeType;
        public abstract IUpgrade GetUpgrade();
    }

    public enum SkillTreeUpgradeType
    {
        Normal,
        Milestone
    }
}
