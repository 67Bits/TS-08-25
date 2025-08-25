using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewUnlockNewDroneUpgrade", menuName = "ScriptableObject/Upgrades/UnlockNewDrone")]
    public class UnlockDroneUpgrade : UpgradeBase
    {
        //public DroneBase.DroneType droneType;
        public override void ApplyUpgrade(Transform t)
        {
            //switch (upgradeType)
            //{
            //    case UpgradeType.InLevel:
            //        Debug.LogError("Cant unlock drone in level");
            //        break;
            //    case UpgradeType.Permanent:
            //        PlayerUpgrader.Instance.UnlockDrone(droneType);
            //        break;
            //}
        }

        public override void RemoveUpgrade(Transform t)
        {

        }
    }
}