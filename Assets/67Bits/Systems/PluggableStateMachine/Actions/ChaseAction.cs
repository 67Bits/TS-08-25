using UnityEngine;

[CreateAssetMenu(fileName = "Chase Action", menuName = "67Bits/Pluggable State Machine/Actions/Chase")]
public class ChaseAction : StateControllerAction
{
    public override void Act(StateController controller)
    {
        Chase(controller);
    }

    private void Chase(StateController controller)
    {
        controller.ChaseTarget();
    }
}