using System;
using System.Collections;
using UnityEngine;
using static Talents.PermanentUpgradeApplier;

namespace Talents
{
    public class TalentsUI : MonoBehaviour
    {
        [Header("references")]
        public PermanentUpgradeApplier permanetUpgrade = null;
        private ICurrency _currency;
        public void OnPushSpinPerks() => StartCoroutine(RollSlotsPerks());
        private IEnumerator RollSlotsPerks()
        {
            Tuple<UpgradeBase, int> upgAndMoney = permanetUpgrade.GetNextUpgrade();
            TierType tier = upgAndMoney.Item1.rarity;

            // Spend Currency
            _currency?.AddCurrency(-upgAndMoney.Item2);

            // Reward
            yield return new WaitForSeconds(1f);

            permanetUpgrade.ApplyUpgrade(upgAndMoney.Item1);
        }
    }
}