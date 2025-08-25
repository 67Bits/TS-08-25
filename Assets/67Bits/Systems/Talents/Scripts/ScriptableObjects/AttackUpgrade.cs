using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewAttackUpgrade", menuName = "ScriptableObject/Upgrades/AttackUpgrade")]
    public class AttackUpgrade : UpgradeBase
    {
        [Range(0, 1)] public float damageUpgradeIncrease;
        public override void ApplyUpgrade(Transform t)
        {
            switch (upgradeType)
            {
                case UpgradeType.InLevel:
                    PlayerUpgrader.Instance.IncreaseDamageFactor(damageUpgradeIncrease);
                    break;
                case UpgradeType.Permanent:
                    PlayerUpgrader.Instance.IncreasePermanentDamageFactor(damageUpgradeIncrease);
                    break;
            }
        }

        public override void RemoveUpgrade(Transform t)
        {

        }
    }
}

