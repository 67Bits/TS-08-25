using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Turret
{
    public class LookToSensor : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private TurretSensor _sensor;
        [SerializeField] private Vector3 _upwards = Vector3.up;

        [Header("Offset")]
        [SerializeField] private Vector3 _eulerAnglesOffset;
        [SerializeField] private Vector3 _targetOffset = Vector3.up;
        [SerializeField] private Transform _offsetDirection;

        [Header("Constraints")]
        [SerializeField] private bool _ignoreX;
        [SerializeField] private bool _ignoreY;
        [SerializeField] private bool _ignoreZ;

        private Vector3 _currentTargetOffset;

        public Transform Target => _target != null ? _target : transform;

        private void LateUpdate()
        {
            // Get the nearest target from sensor
            GameObject targetObject = _sensor?.CurrentTarget;
            if (targetObject == null) return;

            Collider targetCollider = targetObject.GetComponent<Collider>();
            if (targetCollider == null) return;

            _currentTargetOffset = _targetOffset;

            // Calculate target position with offset
            Transform offsetDirection = _offsetDirection != null ? _offsetDirection : targetCollider.transform;
            Vector3 targetPosition = targetCollider.transform.position + offsetDirection.TransformVector(_currentTargetOffset);

            // Get rotation towards target
            Quaternion rotation = GetRotation(targetPosition);

            // Apply Euler angles offset
            rotation *= Quaternion.Euler(_eulerAnglesOffset);

            // Apply rotation
            Target.rotation = rotation;

            // Apply axis constraints
            Vector3 eulerAngles = ClampEulerAngles(Target.localRotation.eulerAngles);
            Target.localRotation = Quaternion.Euler(eulerAngles);
        }

        private Vector3 ClampEulerAngles(Vector3 eulerAngles)
        {
            if (_ignoreX) eulerAngles.x = 0;
            if (_ignoreY) eulerAngles.y = 0;
            if (_ignoreZ) eulerAngles.z = 0;
            return eulerAngles;
        }

        protected virtual Quaternion GetRotation(Vector3 target)
        {
            Vector3 forward = (target - transform.position).normalized;
            return Quaternion.LookRotation(forward, _upwards);
        }
    }
}