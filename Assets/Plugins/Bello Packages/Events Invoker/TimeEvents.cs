using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Bello.EventsInvoker
{
    public class TimeEvents : MonoBehaviour
    {
        [SerializeField] private InitializationType _initializationType;
        [HorizontalGroup("Timer"),SerializeField] private float _time;
        [HorizontalGroup("Timer"), SerializeField] private float _timerDelay;
        [SerializeField] private bool _loop;
        [HorizontalGroup("Loop"),ShowIf("@_loop"), SerializeField] private int _loopTarget;
        [HorizontalGroup("Loop"),ShowIf("@_loop"), SerializeField] private int _loopCount;
        [FoldoutGroup("Events"), SuffixLabel("Runs on start")] public UnityEvent OnStartTimer;
        [FoldoutGroup("Events"), SuffixLabel("Runs after timer delay")] public UnityEvent OnStartWait;
        [FoldoutGroup("Events"), SuffixLabel("Runs after timer")] public UnityEvent OnFinishWait;
        [FoldoutGroup("Events"), SuffixLabel("Runs on stop")] public UnityEvent OnStopWait;
        [FoldoutGroup("Events"), SuffixLabel("Runs after fully complete the timer")] public UnityEvent OnCompleteAll;
        private void Start()
        {
            switch (_initializationType)
            {
                case InitializationType.OnStart:
                case InitializationType.Both:
                    StartTimer();
                    break;
            }
        }
        private void OnEnable()
        {
            switch (_initializationType)
            {
                case InitializationType.OnEnable:
                case InitializationType.Both:
                    StartTimer();
                    break;
            }
        }
        public void StartTimer()
        {
            OnStartTimer?.Invoke();
            StartCoroutine(TimerRoutine());
        }
        private IEnumerator TimerRoutine()
        {
            yield return new WaitForSeconds(_timerDelay);
            OnStartWait?.Invoke();
            do
            {
                yield return new WaitForSeconds(_time);
                OnFinishWait?.Invoke();
                _loopCount++;
            } while (_loop && (_loopTarget == 0 || _loopCount <= _loopTarget));
            OnCompleteAll?.Invoke();
        }
        public void StopTimer()
        {
            OnStopWait?.Invoke();
            StopAllCoroutines();
        }
        private enum InitializationType
        {
            Script,
            OnStart,
            OnEnable,
            Both,
        }
    }
}
