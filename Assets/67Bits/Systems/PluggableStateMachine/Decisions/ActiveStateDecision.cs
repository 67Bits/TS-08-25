using UnityEngine;
[CreateAssetMenu(fileName = "Active State Decision", menuName = "67Bits/Pluggable State Machine/Decisions/ActiveState")]
public class ActiveStateDecision : Decision
{
    public override bool Decide(StateController controller)
    {
        bool chaseTargetIsActive = controller.TargetIsActive;
        return chaseTargetIsActive;
    }
}