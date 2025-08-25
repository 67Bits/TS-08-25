using System;
using System.Collections.Generic;
using UnityEngine;

namespace KainCobra.Talents
{
    public abstract class TalentBase : ScriptableObject
    {
        public string Id;
        public string Name = "";
        [TextArea] public string Description = "";
        public Sprite Icon;
        public Sprite Background;
        public enum UpgradeType
        {
            InLevel,
            Permanent
        }
        public UpgradeType Type = UpgradeType.Permanent;
        public Rarity Rarity = Rarity.Common;
        public List<TalentLevel> TalentLevels;
        public List<TalentBase> ChainedUpgrades = new List<TalentBase>();
        public int StackableCount = 1;
        [field: SerializeField, ReadOnly] public TalentData TalentData { get; set; }
        [field: SerializeField, ReadOnly] public bool IsApplied { get; set; } = false;
        public virtual void ApplyTalent<T>(T receiver){ }
        public bool CanApply()
        {
            if (Type == UpgradeType.Permanent && IsApplied)
                return false;
            if (Type == UpgradeType.Permanent)
                IsApplied = true;

            return true;
        }
        private void OnValidate()
        {
            if (String.IsNullOrEmpty(Id))
                GenerateId();
        }
        [ContextMenu("Generate Id")]
        public void GenerateId() => Id = Guid.NewGuid().ToString();
    }

    public enum Rarity
    {
        Common,
        Rare,
        UltraRare,
        M
    }

    [Serializable]
    public class TalentLevel
    {
        [HideInInspector] public string Name;
        public int Level;
        public float ValueEffect;
    }
    [Serializable]
    public class TalentData
    {
        public string IdTalent;
        public int Level;

        public TalentData(string idTalent, int level)
        {
            IdTalent = idTalent;
            Level = level;
        }
    }
}