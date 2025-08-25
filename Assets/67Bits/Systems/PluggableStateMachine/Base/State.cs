using UnityEngine;

[CreateAssetMenu(menuName = "67Bits/Pluggable State Machine/State")]
public class State : ScriptableObject
{
    [SerializeField] private StateControllerAction[] _actions;
    [SerializeField] private Transition[] _transitions;
    [SerializeField] private StateBuff[] _buffs;
    [SerializeField] private Color _sceneGizmoColor;

    public Color SceneGizmoColor => _sceneGizmoColor;

    public void UpdateState(StateController controller)
    {
        DoActions(controller);
        CheckTransitions(controller);
    }

    private void DoActions(StateController controller)
    {
        for (int i = 0; i < _actions.Length; i++)
        {
            _actions[i].Act(controller);
        }
    }

    private void CheckTransitions(StateController controller)
    {
        for (int i = 0; i < _transitions.Length; i++)
        {
            bool decision = _transitions[i].Decision.Decide(controller);
            if (decision)
            {
                controller.TransitionToState(_transitions[i].TrueState);
            }else
            {
                controller.TransitionToState(_transitions[i].FalseState);
            }
        }
    }

    public void ApplyBuffs(StateController controller)
    {
        for (int i = 0; i < _buffs.Length; i++)
        {
            _buffs[i].ApplyBuff(controller);
        }
    }

    public void RemoveBuffs(StateController controller)
    {
        for (int i = 0; i < _buffs.Length; i++)
        {
            _buffs[i].RemoveBuff(controller);
        }
    }
}