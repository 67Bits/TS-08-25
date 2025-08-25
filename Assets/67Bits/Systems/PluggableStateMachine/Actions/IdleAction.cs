using UnityEngine;

[CreateAssetMenu(fileName = "Idle Action", menuName = "67Bits/Pluggable State Machine/Actions/Idle")]
public class IdleAction : StateControllerAction
{
    public override void Act(StateController controller)
    {
        controller.StopMovement();
    }
}