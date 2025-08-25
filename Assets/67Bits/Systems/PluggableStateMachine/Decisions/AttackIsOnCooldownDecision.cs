using UnityEngine;

[CreateAssetMenu(fileName = "Attack Is On Cooldown Decision", menuName = "67Bits/Pluggable State Machine/Decisions/AttackIsOnCooldown")]

public class AttackIsOnCooldownDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        var isOnCooldown = GetIsOnAttackCooldown(controller);
        return isOnCooldown;
    }

    private bool GetIsOnAttackCooldown(StateController controller)
    {
        return controller.IsOnAttackCooldown;
    }
}
