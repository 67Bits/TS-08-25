using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewLuckyUpgrade", menuName = "ScriptableObject/Upgrades/LuckyUpgrade")]
    public class LuckyUpgrade : UpgradeBase
    {
        public float amountToIncreaseLegendaryChance = 0.005f;
        public override void ApplyUpgrade(Transform t)
        {
            PlayerUpgrader.Instance.IncreaseLegendaryChance(amountToIncreaseLegendaryChance);
        }

        public override void RemoveUpgrade(Transform t)
        {

        }

    }
}