using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSBTutorial
{
    [CreateAssetMenu(fileName = "Default Step", menuName = "67Bits/Tutorial/New Step", order = 0)]
    public class TutorialStep : ScriptableObject
    {
        [ReadOnly, SerializeField] private TutorialStepStatus _status;
        [SerializeField, Tooltip("Use this to describe this step of the tutorial, for example where it triggers and where it completes")] private string _description;
        [SerializeField] private bool _debugLogs;
        [SerializeField] private bool _canBeForcedAll;
        [SerializeField] private bool _canBeForcedEnabled;
        [SerializeField] private bool _requireEnableToComplete;
        [FoldoutGroup("Events")] public UnityEvent OnEnable;
        [FoldoutGroup("Events")] public UnityEvent OnDisable;
        [FoldoutGroup("Events")] public UnityEvent OnComplete;

        private string _key => $"{name} tutorial";
        private List<ITutorial> _stepObjects = new List<ITutorial>();
        public TutorialStepStatus Status => _status;
        public void AddObject(ITutorial objectToAdd)
        {
            if (!_stepObjects.Contains(objectToAdd))
            {
                _stepObjects.Add(objectToAdd);
            }
        }
        public void RemoveObject(ITutorial objectToRemove)
        {
            _stepObjects.Remove(objectToRemove);
        }
        public void ActiveTutorial()
        {
            if (!_canBeForcedEnabled && !_canBeForcedAll && _status == TutorialStepStatus.Completed)
                return;

            SetStatus(TutorialStepStatus.Enabled);
            CallEvent();
        }
        public void CompleteTutorial()
        {
            if (!_canBeForcedAll && _status == TutorialStepStatus.Completed ||
                _requireEnableToComplete && _status != TutorialStepStatus.Enabled)
                return;

            SetStatus(TutorialStepStatus.Completed);
            CallEvent();
        }
        public void DisableTutorial()
        {
            if (!_canBeForcedAll && _status == TutorialStepStatus.Completed)
                return;

            SetStatus(TutorialStepStatus.Disabled);
            CallEvent();
        }
        [Button]
        public void ForceActiveTutorial()
        {
            SetStatus(TutorialStepStatus.Enabled);
            CallEvent();
        }
        [Button]
        public void ForceCompleteTutorial()
        {
            SetStatus(TutorialStepStatus.Completed);
            CallEvent();
        }
        [Button]
        public void ForceDisableTutorial()
        {
            SetStatus(TutorialStepStatus.Disabled);
            CallEvent();
        }
        private void SetStatus(TutorialStepStatus statusToSet)
        {
            _status = statusToSet;
            SaveState();
        }
        private void CallEvent()
        {
            switch (_status)
            {
                case TutorialStepStatus.Disabled:
                    OnDisable?.Invoke();
                    break;
                case TutorialStepStatus.Enabled:
                    OnEnable?.Invoke();
                    break;
                case TutorialStepStatus.Completed:
                    OnComplete?.Invoke();
                    break;
            }
            for (int i = 0; i < _stepObjects.Count; i++)
            {
                ITutorial obj = _stepObjects[i];
                switch (_status)
                {
                    case TutorialStepStatus.Disabled:
                        obj.OnTutorialDisabled?.Invoke();
                        break;
                    case TutorialStepStatus.Enabled:
                        obj.OnTutorialEnabled?.Invoke();
                        break;
                    case TutorialStepStatus.Completed:
                        obj.OnTutorialCompleted?.Invoke();
                        break;
                }
                if (_debugLogs && obj is MonoBehaviour mono)
                {
                    Debug.Log($"Calling event {_status} on {mono.name}");
                }
                obj.CheckStepCurrentStatus();
            }
        }
        private void SaveState()
        {
            PlayerPrefs.SetInt(_key, (int)_status);
        }
        public void LoadState()
        {
            int state = PlayerPrefs.GetInt(_key, 0);
            _status = (TutorialStepStatus)state;
        }
        public void Reset()
        {
            PlayerPrefs.SetInt(_key, 0);
            _status = TutorialStepStatus.Disabled;
        }
    }
    public enum TutorialStepStatus
    {
        Disabled,
        Enabled,
        Completed
    }
}