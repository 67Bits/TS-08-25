using UnityEngine;

[CreateAssetMenu(fileName = "Attack Target Range Decision", menuName = "67Bits/Pluggable State Machine/Decisions/AttackTargetOnRange")]
public class AttackTargetOnRangeDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        bool canAttack = controller.HasTargetOnRange();
        return canAttack;
    }
}