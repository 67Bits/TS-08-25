using UnityEngine;

public abstract class StateBuff : ScriptableObject
{
    public abstract void ApplyBuff(StateController controller);
    public abstract void RemoveBuff(StateController controller);
}