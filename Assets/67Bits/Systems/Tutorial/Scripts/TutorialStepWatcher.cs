using SSBTutorial;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSBTutorial
{
    public class TutorialStepWatcher : MonoBehaviour, ITutorial
    {
        [SerializeField] private TutorialStep _stepToWatch;
        [SerializeField] private TutorialStepStatus _statusToWatch;

        [SerializeField] private UnityEvent _onStatusAchieved;

        public UnityEvent OnTutorialDisabled { get; set; }
        public UnityEvent OnTutorialEnabled { get; set; }
        public UnityEvent OnTutorialCompleted { get; set; }


        private void OnStatusAchieved()
        {
            _onStatusAchieved?.Invoke();
        }

        public void CheckStepCurrentStatus()
        {
            if (_stepToWatch.Status == _statusToWatch)
                OnStatusAchieved();
        }

        public void OnDestroy()
        {
            _stepToWatch.RemoveObject(this);
        }

        public void Awake()
        {
            _stepToWatch.AddObject(this);
            CheckStepCurrentStatus();
        }
    }
}
