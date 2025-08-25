using UnityEngine;

[CreateAssetMenu(fileName = "Is At Follow Position Decision", menuName = "67Bits/Pluggable State Machine/Decisions/IsAtFollowPosition")]
public class IsAtFollowPositionDecision : Decision
{
    [SerializeField] private float _distanceToSetIsAtFollowPos = .1f;
    public override bool Decide(StateController controller)
    {
        bool isAtFollowPos = CheckIsAtFollowPosition(controller);
        return isAtFollowPos;
    }

    private bool CheckIsAtFollowPosition(StateController controller)
    {
        return controller.DistanceToFollowTarget < _distanceToSetIsAtFollowPos;
    }
}