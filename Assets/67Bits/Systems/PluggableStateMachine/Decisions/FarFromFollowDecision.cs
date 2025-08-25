using UnityEngine;

[CreateAssetMenu(fileName = "Far From Follow Decision", menuName = "67Bits/Pluggable State Machine/Decisions/FarFromFollowDecision")]
public class FarFromFollowDecision : Decision
{
    [SerializeField] private float _distanceToReturnToFollow = 5;
    [SerializeField] private Color _gizmosColor;
    public override bool Decide(StateController controller)
    {
        bool isFar = GetDistanceFromFollowTarget(controller);

        return isFar;
    }

    private bool GetDistanceFromFollowTarget(StateController controller)
    {
        return controller.DistanceToFollowTarget > _distanceToReturnToFollow;
    }
}