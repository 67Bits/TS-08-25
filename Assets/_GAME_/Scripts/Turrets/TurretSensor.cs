using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SSBHealthSystem;
using System;
using Random = UnityEngine.Random;
using Unity.Collections;


namespace Turret
{
    public class TurretSensor : MonoBehaviour
    {
        [Serializable]
        public enum TargetMode
        {
            Closest,
            Farthest,
            Random
        }
        [Header("Current Target")]
        [ReadOnly] public GameObject CurrentTarget;

        [Header("Sensor")]
        [SerializeField] private TargetMode _targetMode = TargetMode.Closest;
        [SerializeField] private float _detectRadius = 5f;
        [SerializeField] private LayerMask _detectLayer;
        [SerializeField, Range(0f, 360f)] private float _angle = 360f;
        [SerializeField] private float _detectDelay;

        private float _time;
        private Collider[] _colliderBuffer = new Collider[20]; // Buffer for physics checks

        private void OnDrawGizmos()
        {
            // Draw detection radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectRadius);

            // Draw angle boundaries if angle is less than 360
            if (_angle < 360f)
            {
                Vector3 rightBoundary = Quaternion.Euler(0, _angle / 2, 0) * transform.forward;
                Vector3 leftBoundary = Quaternion.Euler(0, -_angle / 2, 0) * transform.forward;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + rightBoundary * _detectRadius * 1.1f);
                Gizmos.DrawLine(transform.position, transform.position + leftBoundary * _detectRadius * 1.1f);

                // Draw arc for better visualization
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
                DrawAngleArc();
            }
        }

        private void DrawAngleArc()
        {
            // Draw a wire arc to visualize the angle
            Vector3 forward = transform.forward;
            Vector3 startDir = Quaternion.Euler(0, -_angle / 2, 0) * forward;

            for (int i = 0; i <= 20; i++)
            {
                float t = i / 20f;
                float currentAngle = Mathf.Lerp(-_angle / 2, _angle / 2, t);
                Vector3 currentDir = Quaternion.Euler(0, currentAngle, 0) * forward;

                if (i > 0)
                {
                    float prevAngle = Mathf.Lerp(-_angle / 2, _angle / 2, (i - 1) / 20f);
                    Vector3 prevDir = Quaternion.Euler(0, prevAngle, 0) * forward;
                    Gizmos.DrawLine(
                        transform.position + prevDir * _detectRadius,
                        transform.position + currentDir * _detectRadius
                    );
                }
            }
        }

        protected virtual void OnEnable()
        {
            _time = 0f;
        }

        protected virtual void FixedUpdate()
        {
            if (_time < _detectDelay)
            {
                _time += Time.fixedDeltaTime;
                return;
            }

            // Reset timer after delay
            _time = 0f;
            CurrentTarget = GetDetectableTarget();
        }

        public bool CanDetect(Collider targetCollider)
        {
            // Check if the object is inside the camera viewport
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 viewportPos = cam.WorldToViewportPoint(targetCollider.transform.position);

                // If behind camera or outside screen
                if (viewportPos.z < 0f || viewportPos.x < 0f || viewportPos.x > 1f || viewportPos.y < 0f || viewportPos.y > 1f)
                {
                    return false;
                }
            }

            // Check health component
            Health health = targetCollider.GetComponent<Health>();
            if (health && (!health.enabled || health.CurrentHealth <= 0))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the detectable GameObject based on the selected target mode
        /// </summary>
        public GameObject GetDetectableTarget()
        {
            List<GameObject> detectableTargets = GetAllDetectableTargets();

            if (detectableTargets.Count == 0)
                return null;

            return _targetMode switch
            {
                TargetMode.Closest => GetClosestTarget(detectableTargets),
                TargetMode.Farthest => GetFarthestTarget(detectableTargets),
                TargetMode.Random => GetRandomTarget(detectableTargets),
                _ => GetClosestTarget(detectableTargets)
            };
        }

        /// <summary>
        /// Returns the nearest detectable GameObject within range, considering angle constraints
        /// </summary>
        public GameObject GetNearestDetectableTarget()
        {
            return GetDetectableTarget(); // For backward compatibility
        }

        private GameObject GetClosestTarget(List<GameObject> targets)
        {
            GameObject closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        private GameObject GetFarthestTarget(List<GameObject> targets)
        {
            GameObject farthestTarget = null;
            float farthestDistance = 0f;

            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance > farthestDistance)
                {
                    farthestDistance = distance;
                    farthestTarget = target;
                }
            }

            return farthestTarget;
        }

        private GameObject GetRandomTarget(List<GameObject> targets)
        {
            if (targets.Count == 0) return null;
            int randomIndex = Random.Range(0, targets.Count);
            return targets[randomIndex];
        }

        /// <summary>
        /// Checks if a target is within the detection angle
        /// </summary>
        private bool IsWithinAngle(Transform target)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            return angleToTarget <= _angle / 2f;
        }

        /// <summary>
        /// Gets all detectable targets within range
        /// </summary>
        public List<GameObject> GetAllDetectableTargets()
        {
            List<GameObject> detectableTargets = new List<GameObject>();

            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                _detectRadius,
                _colliderBuffer,
                _detectLayer
            );

            for (int i = 0; i < hitCount; i++)
            {
                Collider collider = _colliderBuffer[i];

                if (collider.gameObject == gameObject)
                    continue;

                if (_angle < 360f && !IsWithinAngle(collider.transform))
                    continue;

                if (CanDetect(collider))
                {
                    detectableTargets.Add(collider.gameObject);
                }
            }

            return detectableTargets;
        }

        // Public methods to change target mode dynamically
        public void SetTargetMode(TargetMode newMode) => _targetMode = newMode;
        public void SetTargetModeClosest() => _targetMode = TargetMode.Closest;
        public void SetTargetModeFarthest() => _targetMode = TargetMode.Farthest;
        public void SetTargetModeRandom() => _targetMode = TargetMode.Random;
    }
}