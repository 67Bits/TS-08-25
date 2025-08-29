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

        [Header("Rotation Settings")]
        [SerializeField] private float _rotationSpeed = 5f;
        [SerializeField] private bool _useGlobalRotation = true;

        [Header("Offset")]
        [SerializeField] private Vector3 _eulerAnglesOffset;
        [SerializeField] private Vector3 _targetOffset = Vector3.up;
        [SerializeField] private Transform _offsetDirection;

        [Header("Constraints")]
        [SerializeField] private bool _ignoreX;
        [SerializeField] private bool _ignoreY;
        [SerializeField] private bool _ignoreZ;

        private Vector3 _currentTargetOffset;
        private Quaternion _targetRotation;

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

            // Get target rotation towards target
            _targetRotation = GetRotation(targetPosition);

            // Apply Euler angles offset
            _targetRotation *= Quaternion.Euler(_eulerAnglesOffset);

            // Apply axis constraints
            Vector3 constrainedEulerAngles = ClampEulerAngles(_targetRotation.eulerAngles);
            _targetRotation = Quaternion.Euler(constrainedEulerAngles);

            // Smoothly lerp towards target rotation
            if (_useGlobalRotation)
            {
                Target.rotation = Quaternion.Slerp(Target.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);
            }
            else
            {
                Target.localRotation = Quaternion.Slerp(Target.localRotation, _targetRotation, _rotationSpeed * Time.deltaTime);
            }
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

        // Optional: For debugging or external access
        public Quaternion GetCurrentTargetRotation()
        {
            return _targetRotation;
        }

        public float GetRotationSpeed()
        {
            return _rotationSpeed;
        }

        public void SetRotationSpeed(float speed)
        {
            _rotationSpeed = Mathf.Max(0, speed);
        }
    }
}