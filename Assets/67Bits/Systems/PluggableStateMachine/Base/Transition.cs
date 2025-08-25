using UnityEngine;

[System.Serializable]
public class Transition
{
    [SerializeField] private Decision _decision;
    [SerializeField] private State _trueState;
    [SerializeField] private State _falseState;
    public Decision Decision => _decision;
    public State TrueState => _trueState;
    public State FalseState => _falseState;
}