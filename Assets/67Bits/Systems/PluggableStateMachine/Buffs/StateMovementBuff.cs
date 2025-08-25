using UnityEngine;

[CreateAssetMenu(fileName = "State Movement Buff", menuName = "67Bits/Pluggable State Machine/Buffs/MovementSpeed")]
public class StateMovementBuff : StateBuff
{
    [SerializeField, Range(0,100)] private int _speedBuffPercentage;
    public override void ApplyBuff(StateController controller)
    {
        controller.IncreaseSpeed(_speedBuffPercentage);
    }

    public override void RemoveBuff(StateController controller)
    {
        controller.DecreaseSpeed(_speedBuffPercentage);
    }
}