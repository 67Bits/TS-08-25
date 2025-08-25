using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewMainWeaponUpgrade", menuName = "ScriptableObject/Upgrades/MainWeaponUpgrade")]

    public class MainWeaponUpgrade : UpgradeBase
    {
        public int mainWeaponQntToIncrease;
        public override void ApplyUpgrade(Transform t)
        {
            switch (upgradeType)
            {
                case UpgradeType.InLevel:
                    PlayerUpgrader.Instance.IncreaseShotQuantity(mainWeaponQntToIncrease);
                    break;
                case UpgradeType.Permanent:
                    PlayerUpgrader.Instance.IncreasePermanentShotQuantity(mainWeaponQntToIncrease);
                    break;
            }
        }

        public override void RemoveUpgrade(Transform t)
        {

        }
    }
}
