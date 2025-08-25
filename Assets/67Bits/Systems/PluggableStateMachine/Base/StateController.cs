using System;
using UnityEngine;

public abstract class StateController : MonoBehaviour
{
    [SerializeField] protected State _currentState;
    [SerializeField] protected State _remainState;
    [SerializeField] protected Transform _eyes;

    protected bool _aiActive = true;

    public Transform Eyes => _eyes;
    public float LookRange => GetLookRange();
    public float AttackRange => GetAttackRange();
    public LayerMask TargetLayerMask => GetTargetLayerMask();
    public bool TargetIsActive => GetTargetIsActive();
    public Transform TransformToFollow { get; set; }
    public Vector3 FollowOffset { get; set; }
    public float DistanceToFollowTarget => GetDistanceToFollowTarget();
    public bool IsOnAttackCooldown => GetIsOnAttackCooldown();

    protected virtual void Awake()
    {
        SetComponents();
    }

    protected virtual void OnEnable()
    {
        _aiActive = true;
    }

    protected virtual void Update()
    {
        UpdateState();
    }

    protected virtual void UpdateState()
    {
        if (!_aiActive)
            return;

        _currentState.UpdateState(this);
    }

    #region Movement
    public abstract void MoveToDirection(Vector3 direction);
    public abstract void MoveToPosition(Vector3 position);
    public abstract void SetTargetToChase(Transform target);
    public abstract void ChaseTarget();
    public abstract void StopMovement();
    public abstract Vector3 GetPatrolDirection();
    public abstract Vector3 GetPatrolTargetPosition();
    public abstract void FollowTarget();
    public abstract void StopFollow();
    public abstract float GetDistanceToFollowTarget();
    public abstract void IncreaseSpeed(int percentage);
    public abstract void DecreaseSpeed(int percentage);

    #endregion

    #region Attack
    public abstract bool HasTargetOnRange();
    public abstract void Attack(/*Character target*/);
    public abstract LayerMask GetTargetLayerMask();
    protected abstract bool GetIsOnAttackCooldown();

    #endregion

    #region Stats
    protected abstract float GetAttackRange();
    protected abstract float GetLookRange();
    protected abstract bool GetTargetIsActive();
    #endregion

    #region States
    public void TransitionToState(State nextState)
    {
        if (_remainState != nextState)
        {
            _currentState.RemoveBuffs(this);
            _currentState = nextState;
            _currentState.ApplyBuffs(this);
            OnExitState();
        }
    }

    protected virtual void OnExitState()
    {

    }
    #endregion

    protected abstract void SetComponents();

    protected abstract void OnDeath();

    private void OnDrawGizmos()
    {
        if (_currentState != null)
        {
            Gizmos.color = _currentState.SceneGizmoColor;
            Gizmos.DrawWireSphere(_eyes.position, 1);
        }
    }
}

public struct WireSphereData
{
    private Color _color;
    private float _sphereRadius;
    private float _additionalDistance;
    public Color SphereColor => _color;
    public float SphereRadius => _sphereRadius;
    public float AdditionalDistance => _additionalDistance;
    public WireSphereData(Color color, float radius)
    {
        _color = color;
        _sphereRadius = radius;
        _additionalDistance = 0;
    }

    public WireSphereData(Color color, float radius, float additionalDistance)
    {
        _color = color;
        _sphereRadius = radius;
        _additionalDistance = additionalDistance;
    }
}