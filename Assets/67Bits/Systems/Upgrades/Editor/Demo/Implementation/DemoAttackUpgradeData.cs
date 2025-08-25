using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;

[System.Serializable]
[CreateAssetMenu(fileName = "new DemoAttackUpgradeData", menuName = "Scriptable Objects/Upgrades/UpgradeData/DemoAttackUpgradeData")]
public class DemoAttackUpgradeData : UpgradeData
{
    public DemoAttackUpgrade upgrade;

    public override IUpgrade GetUpgrade()
        => upgrade;
}
