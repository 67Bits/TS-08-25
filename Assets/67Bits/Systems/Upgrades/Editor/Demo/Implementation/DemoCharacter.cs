using UnityEngine;
using Modifiables;
using Upgrades;
using System.Collections.Generic;

public class DemoCharacter : MonoBehaviour, IUpgradeTarget
{
    #region Attributes

    private ModifiableValue<float> _attackModValue;
    private UpgradesService upgradesService;
    private List<IUpgrade> tempUpgrades;

    public float AttackValue
    {
        get => _attackModValue.Value;
    }

    #endregion

    #region Enable / Disable

    void OnEnable()
    {
        UpgradesService upgradesService = UpgradesService.Instance;

        upgradesService.OnPermanentUpgradeAdded += AddPermanentUpgrade;
        upgradesService.OnTempUpgradeAdded += AddTempUpgrade;
        upgradesService.OnClearAllTempUpgrades += ClearAllTempUpgrades;
    }

    private void OnDisable()
    {
        upgradesService.OnPermanentUpgradeAdded -= AddPermanentUpgrade;
        upgradesService.OnTempUpgradeAdded -= AddTempUpgrade;
    }

    #endregion
    
    #region Modification

    public void AddPermanentAttackModification(Modification<float> modification)
    {
        _attackModValue.GetModification(modification);
    }

    public void AddTempAttackModification(Modification<float> modification)
    {
        _attackModValue.GetModification(modification);
    }

    public void ClearAttackModification(Modification<float> modification)
    {
        _attackModValue.RemoveModification(modification);
    }

    #endregion

    #region Upgrades

    public void AddPermanentUpgrade(IUpgrade upgrade)
    {
        upgrade.ApplyUpdate(this);
    }

    public void AddTempUpgrade(IUpgrade upgrade)
    {
        tempUpgrades.Add(upgrade);
        upgrade.ApplyUpdate(this);
    }

    public void ClearAllTempUpgrades()
    {
        foreach (var upgrade in tempUpgrades)
        {
            upgrade.ClearUpdate(this);
        }
    }

    #endregion
}
