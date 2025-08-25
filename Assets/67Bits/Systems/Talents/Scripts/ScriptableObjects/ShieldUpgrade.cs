using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewShieldUpgrade", menuName = "ScriptableObject/Upgrades/ShieldUpgrade")]
    public class ShieldUpgrade : UpgradeBase
    {
        public override void ApplyUpgrade(Transform t)
        {
            //EntityStatus status = t.GetComponent<EntityStatus>();

            //if (status)
            //{
            //    switch (upgradeType)
            //    {
            //        case UpgradeType.InLevel:
            //            //status.ShieldUpgrade(true);
            //            break;
            //        case UpgradeType.Permanent:
            //            PlayerUpgrader.Instance.UnlockShieldOnStart();
            //            //status.ShieldUpgrade(true);
            //            break;
            //    }
            //}
        }

        public override void RemoveUpgrade(Transform t)
        {

        }
    }
}
