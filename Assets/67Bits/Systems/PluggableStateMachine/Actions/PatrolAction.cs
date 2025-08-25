using UnityEngine;

[CreateAssetMenu(fileName = "Patrol Action", menuName = "67Bits/Pluggable State Machine/Actions/Patrol")]
public class PatrolAction : StateControllerAction
{
    public override void Act(StateController controller)
    {
        Patrol(controller);
    }

    private void Patrol(StateController controller)
    {
        var position = controller.GetPatrolTargetPosition();
        controller.MoveToPosition(position);
    }
}