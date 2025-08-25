using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewCriticalHitUpgrade", menuName = "ScriptableObject/Upgrades/CriticalHitUpgrade")]
    public class CriticalHitUpgrade : UpgradeBase
    {
        public float criticalChanceAmount = 0.1f;
        public override void ApplyUpgrade(Transform t)
        {
            switch (upgradeType)
            {
                case UpgradeType.InLevel:
                    PlayerUpgrader.Instance.IncreaseCriticalHitChance(criticalChanceAmount);
                    break;
                case UpgradeType.Permanent:
                    PlayerUpgrader.Instance.IncreasePermanentCriticalHitChance(criticalChanceAmount);
                    break;
            }
        }

        public override void RemoveUpgrade(Transform t)
        {

        }
    }
}