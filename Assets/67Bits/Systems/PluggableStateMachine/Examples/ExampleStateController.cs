using System;
using UnityEngine;

public class ExampleStateController : StateController
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _lookRange;
    [SerializeField] private float _atkRange;
    [SerializeField] private float _atkCooldown;
    [SerializeField] private LayerMask _targetLayerMask;

    [Header("Patrol Positions")]
    [SerializeField] private Transform[] _patrolPositions;
    private int _patrolIndex;

    private Transform _chaseTarget;
    private float _lastAttackTime;

    #region Attack
    public override void Attack()
    {
        Debug.Log("attacking");
        _lastAttackTime = Time.time;
    }

    public override bool HasTargetOnRange()
    {
        return Vector3.Distance(transform.position, _chaseTarget.position) <= _atkRange;
    }

    public override LayerMask GetTargetLayerMask()
    {
        return _targetLayerMask;
    }

    protected override float GetAttackRange()
    {
        return _atkRange;
    }

    protected override bool GetIsOnAttackCooldown()
    {
        bool isOnCooldown = Time.time < _lastAttackTime + _atkCooldown;
        return isOnCooldown;
    }

    #endregion

    #region Movement
    public override void SetTargetToChase(Transform target)
    {
        _chaseTarget = target;
    }

    public override void ChaseTarget()
    {
        MoveToPosition(_chaseTarget.position);
    }

    public override void DecreaseSpeed(int percentage)
    {
        throw new System.NotImplementedException();
    }

    public override void FollowTarget()
    {
        throw new System.NotImplementedException();
    }

    public override float GetDistanceToFollowTarget()
    {
        return 0;
    }

    public override Vector3 GetPatrolDirection()
    {
        return Vector3.zero;
    }

    public override Vector3 GetPatrolTargetPosition()
    {
        return _patrolPositions[_patrolIndex].position;
    }

    public override void IncreaseSpeed(int percentage)
    {
        throw new System.NotImplementedException();
    }

    public override void MoveToDirection(Vector3 direction)
    {
    }

    public override void MoveToPosition(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, _movementSpeed * Time.deltaTime);
    }

    public override void StopFollow()
    {
    }

    public override void StopMovement()
    {
    }

    #endregion

    protected override float GetLookRange()
    {
        return _lookRange;
    }

    protected override bool GetTargetIsActive()
    {
        return true;
    }

    protected override void OnDeath()
    {
    }

    protected override void SetComponents()
    {

    }

    protected override void Update()
    {
        base.Update();

        if (transform.position == _patrolPositions[_patrolIndex].position)
        {
            _patrolIndex++;
            _patrolIndex = (int)Mathf.Repeat(_patrolIndex, _patrolPositions.Length);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _lookRange);
    }
}
