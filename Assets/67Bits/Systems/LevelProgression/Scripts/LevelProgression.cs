using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SSBLevelProgression
{
    public class LevelProgression : MonoBehaviour
    {
        [SerializeField] private List<LevelEventInvoker> _levelEvents;
        [SerializeField] private int _maxPoints = 100;
        [SerializeField, Sirenix.OdinInspector.ReadOnly] private int _progress;
        private int _currentEventId;

        public int CurrentProgress => _progress;
        public int MaxPoints => _maxPoints;
        public List<LevelEventInvoker> LevelEvents => _levelEvents;

        [Header("Events")]
        [field: SerializeField] public UnityEvent OnProgressionChange { get; set; }
        [field: SerializeField] public UnityEvent OnMaxPointsReached { get; set; }

        public void AddProgress(int progressAmount = 1)
        {
            if (_progress >= _maxPoints)
                return;

            _progress += progressAmount;
            _progress = Mathf.Min(_progress, _maxPoints);

            if (_progress >= _maxPoints)
            {
                OnMaxPointsReached?.Invoke();
            }

            OnProgressionChange?.Invoke();
            CheckEvents();
        }

        private void CheckEvents()
        {
            for (int i = 0; i < _levelEvents.Count; i++)
            {
                var levelEvent = _levelEvents[i];
                if (_progress >= levelEvent.PointsToAppear && !levelEvent.Reached)
                {
                    levelEvent.Reached = true;
                    _currentEventId = i;
                }
            }
        }

        private void ResetProgression()
        {
            _progress = 0;

            for (int i = 0; i < _levelEvents.Count; i++)
            {
                _levelEvents[i].Reached = false;
            }
        }


        [Sirenix.OdinInspector.Button]
        public void DebugAddProgression()
        {
            AddProgress(5);
        }

    }

}