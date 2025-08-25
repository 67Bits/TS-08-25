using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSBDrops
{
    public class ItemDropUIMovement : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _movementCurve;
        [SerializeField] private float _movementDuration;

        private RectTransform _rectTransform => transform as RectTransform;
        public float MovementDuration { get => _movementDuration; set => _movementDuration = value; }

        public Action OnArriveOnTargetPos;

        public void GoToPosition(Vector3 worldPosOrigin, Vector2 screenPosTarget)
        {
            _rectTransform.position = Camera.main.WorldToScreenPoint(worldPosOrigin);
            StartCoroutine(MoveToPosition(screenPosTarget));
        }

        private IEnumerator MoveToPosition(Vector2 targetPos)
        {
            float timer = 0;
            float lerpValue = 0;
            Vector3 startPosition = _rectTransform.position;

            while (lerpValue < 1)
            {
                timer += Time.deltaTime;
                lerpValue = timer / _movementDuration;
                _rectTransform.position = Vector3.Lerp(startPosition, targetPos, _movementCurve.Evaluate(lerpValue));
                yield return new WaitForEndOfFrame();
            }

            OnArriveOnTargetPos?.Invoke();
        }
    }
}