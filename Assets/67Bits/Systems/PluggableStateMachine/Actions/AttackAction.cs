using UnityEngine;

[CreateAssetMenu(fileName = "Attack Action", menuName = "67Bits/Pluggable State Machine/Actions/Attack")]
public class AttackAction : StateControllerAction
{
    [SerializeField] private Color _gizmosColor;
    public override void Act(StateController controller)
    {
        Attack(controller);
    }

    private void Attack(StateController controller)
    {
        controller.Attack();
    }
}