using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso
{
    public class Bullet : MonoBehaviour
    {
        public enum MovementType
        {
            TimeBased,
            VelocityBased
        }

        [Header("Movement Settings")]
        public MovementType movementType = MovementType.TimeBased;
        public float duration = 3f;         // Used in TimeBased
        public float velocity = 5f;         // Used in VelocityBased
        public float maxHeight = 5f;
        public float destroyAfterSeconds = 1f;
        public bool lookatTarget = false;
        public Vector3 lookAtOffset = Vector3.zero;

        [Header("Events")]
        public UnityEvent OnReachDestination;

        [Header("Animation Curve")]
        public AnimationCurve heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private float startTime;
        private bool isMoving = false;

        private Transform pointA;
        private Transform pointB;
        public Vector3 offSetPointB = Vector3.up;

        private void Start()
        {
            OnReachDestination.AddListener(() =>
            {
                Destroy(gameObject, destroyAfterSeconds);
            });
        }

        public void SetUpCurvedMovement(Transform startPoint, Transform endPoint, UnityEvent onReachDestination)
        {
            pointA = startPoint;
            pointB = endPoint;
            OnReachDestination.AddListener(onReachDestination.Invoke);
            StartMovement();
        }

        public void StartMovement()
        {
            startTime = Time.time;
            isMoving = true;
        }

        void Update()
        {
            if (!isMoving) return;

            float progress = 0f;
            float elapsed = Time.time - startTime;

            switch (movementType)
            {
                case MovementType.TimeBased:
                    progress = Mathf.Clamp01(elapsed / duration);
                    break;

                case MovementType.VelocityBased:
                    float totalDistance = Vector3.Distance(
                        pointA.position,
                        pointB.position + offSetPointB);
                    float traveled = velocity * elapsed;
                    progress = totalDistance > 0f ? Mathf.Clamp01(traveled / totalDistance) : 1f;
                    break;
            }

            if (progress >= 1f)
            {
                transform.position = pointB.position + offSetPointB;
                isMoving = false;
                OnReachDestination.Invoke();
                return;
            }

            Vector3 horizontalPos = Vector3.Lerp(
                pointA.position,
                pointB.position + offSetPointB,
                progress);

            float height = heightCurve.Evaluate(progress) * maxHeight;

            transform.position = new Vector3(horizontalPos.x, horizontalPos.y + height, horizontalPos.z);

            if (lookatTarget && pointB != null)
            {
                Vector3 targetPosition = pointB.position + offSetPointB + lookAtOffset;
                transform.LookAt(targetPosition);
            }
        }
    }
}
