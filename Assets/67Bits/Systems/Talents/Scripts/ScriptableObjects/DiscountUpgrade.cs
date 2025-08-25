using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewDiscountUpgrade", menuName = "ScriptableObject/Upgrades/DiscountUpgrade")]
    public class DiscountUpgrade : UpgradeBase
    {
        public int jackpotQntToDiscount = 2;
        public override void ApplyUpgrade(Transform t)
        {
            PlayerUpgrader.Instance.IncreaseJackpotDiscount(jackpotQntToDiscount);
            //JackpotManager.Instance.UpdatePullCostText();
        }

        public override void RemoveUpgrade(Transform t)
        {

        }
    }
}