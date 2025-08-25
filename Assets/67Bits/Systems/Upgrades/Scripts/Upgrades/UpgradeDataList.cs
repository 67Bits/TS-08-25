using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;

namespace Upgrades
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "new UpgradeDataList", menuName = "Scriptable Objects/Upgrades/UpgradeDataList")]
    public class UpgradeDataList : ScriptableObject
    {
        [SerializeField] public List<UpgradeData> List = new List<UpgradeData>();
    }
}