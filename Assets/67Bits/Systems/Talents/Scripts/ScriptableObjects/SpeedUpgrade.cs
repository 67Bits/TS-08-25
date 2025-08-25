using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewSpeedUpgrade", menuName = "ScriptableObject/Upgrades/SpeedUpgrade")]
    public class SpeedUpgrade : UpgradeBase
    {
        [Range(0, 1)] public float speedToIncrease;
        public override void ApplyUpgrade(Transform t)
        {
            switch (upgradeType)
            {
                case UpgradeType.InLevel:
                    PlayerUpgrader.Instance.IncreaseAttackSpeedFactor(speedToIncrease);
                    break;
                case UpgradeType.Permanent:
                    PlayerUpgrader.Instance.IncreasePermanentAttackSpeedFactor(speedToIncrease);
                    break;
            }
        }

        public override void RemoveUpgrade(Transform t)
        {

        }
    }
}