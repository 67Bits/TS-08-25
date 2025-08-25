using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.Projectile
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _baseSpeed;
        [SerializeField] private float _timeBeforeDisable = 5;
        [SerializeField, Range(0, 1f)] private float _lerp;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private AnimationCurve _animationHeightForce;
        [SerializeField] private Transform _visual;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;

        [FoldoutGroup("Events")]public UnityEvent OnStartMove;
        [FoldoutGroup("Events")]public UnityEvent OnFinishMove;
        public void Initialize(Vector3 startPoint, Vector3 endPoint, Action onArriveCallback, float? overrideSpeed = null)
        {
            _startPoint.position = startPoint;
            _endPoint.position = endPoint;
            Move(overrideSpeed ?? _baseSpeed, onArriveCallback);
        }
        async void Move(float speed, Action onFinish)
        {
            OnStartMove?.Invoke();
            _lerp = 0f;
            while (_lerp < 1)
            {
                _lerp += Time.deltaTime * speed;
                MoveVisual();
                await Awaitable.EndOfFrameAsync();
            }
            onFinish?.Invoke();
            OnFinishMove?.Invoke();
            await Awaitable.WaitForSecondsAsync(_timeBeforeDisable);
            gameObject?.SetActive(false);
        }
        public void MoveVisual()
        {
            var newPosition = FixedSlerp(_startPoint.position, _endPoint.position, _lerp);
            _visual.forward = newPosition - _visual.position;
            _visual.transform.position = newPosition;
        }
        private Vector3 FixedSlerp(Vector3 start, Vector3 end, float lerpValue)
        {
            var newPosition = Vector3.Lerp(start, end, _animationCurve.Evaluate(lerpValue));
            newPosition += Vector3.up * _animationHeightForce.Evaluate(lerpValue);
            return newPosition;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < 100; i++)
            {
                Gizmos.DrawSphere(FixedSlerp(_startPoint.position, _endPoint.position, i / 100f), 0.05f);
            }
        }
        private void OnValidate()
        {
            MoveVisual();
        }
    }
}
