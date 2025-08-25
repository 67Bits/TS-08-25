using Modifiables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;

[System.Serializable]
public class DemoAttackUpgrade : IUpgrade
{
    public Modification<float> attackModification =
        new Modification<float>(10.0f, Operation.Add);

    public void ApplyUpdate(IUpgradeTarget character)
    {
        if ((character == null) 
            || !character.GetType().IsAssignableFrom(typeof(DemoCharacter)))
            return;

        ((DemoCharacter)character).AddTempAttackModification(attackModification);
    }

    public void ClearUpdate(IUpgradeTarget character)
    {
        if ((character == null)
            || !character.GetType().IsAssignableFrom(typeof(DemoCharacter)))
            return;

        ((DemoCharacter)character).ClearAttackModification(attackModification);
    }

    public string ValueAsText()
    {
        return "+10";
    }
}
