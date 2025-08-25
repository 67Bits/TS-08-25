using UnityEngine;

[CreateAssetMenu(fileName = "Look Decision", menuName = "67Bits/Pluggable State Machine/Decisions/LookDecision")]
public class LookDecision : Decision
{
    [SerializeField] private Color _gizmoColor;
    public override bool Decide(StateController controller)
    {
        bool targetVisible = Look(controller);
        return targetVisible;
    }

    private bool Look(StateController controller)
    {
        var hit = Physics.OverlapSphere(controller.Eyes.position, controller.LookRange, controller.TargetLayerMask);

        if (hit.Length > 0)
        {
            controller.SetTargetToChase(hit[0].transform);
            return true;
        }
        return false;
    }

}