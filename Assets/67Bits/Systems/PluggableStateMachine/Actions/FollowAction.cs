using UnityEngine;

[CreateAssetMenu(fileName = "Follow Action", menuName = "67Bits/Pluggable State Machine/Actions/Follow")]
public class FollowAction : StateControllerAction
{
    public override void Act(StateController controller)
    {
        Follow(controller);
    }

    private void Follow(StateController controller)
    {
        controller.FollowTarget();
    }
}