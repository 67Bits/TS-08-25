using UnityEngine;

public abstract class StateControllerAction : ScriptableObject
{
    public abstract void Act(StateController controller);
}
