using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSBTutorial
{
    public class TutorialObject : MonoBehaviour, ITutorial
    {
        [SerializeField] private TutorialStep _tutorialStep;
        [SerializeField, Tooltip("Should this object deactivate on Awake of the scene?")] private bool _disableObj = true;
        [SerializeField] private bool _setEnableWithStep = true;
        [SerializeField] private bool _disableOnComplete = true;

        [field: SerializeField] public UnityEvent OnTutorialEnabled { get; set; }
        [field: SerializeField] public UnityEvent OnTutorialCompleted { get; set; }
        [field: SerializeField] public UnityEvent OnTutorialDisabled { get; set; }

        public void Awake()
        {
            AssignObjectToTutorialStep();
            if (_disableObj)
            {
                DisableObject();
            }
        }

        private void AssignObjectToTutorialStep()
        {
            if (!_tutorialStep)
            {
                Debug.LogWarning("Tutorial Step is Null!");
                return;
            }

            _tutorialStep.AddObject(this);

            CheckStepCurrentStatus();
        }

        public void CheckStepCurrentStatus()
        {
            switch (_tutorialStep.Status)
            {
                case TutorialStepStatus.Disabled:
                    DisableObject();
                    return;
                case TutorialStepStatus.Enabled:
                    EnableObject();
                    return;
                case TutorialStepStatus.Completed:
                    if (_disableOnComplete)
                        DisableObject();
                    return;
                default:
                    return;
            }
        }

        private void DisableObject()
        {
            if (!_setEnableWithStep)
                return;

            if (_tutorialStep && _tutorialStep.Status != TutorialStepStatus.Enabled)
                gameObject.SetActive(false);
        }

        private void EnableObject()
        {
            if (!_setEnableWithStep)
                return;
            gameObject.SetActive(true);
        }

        public void OnDestroy()
        {
            _tutorialStep.RemoveObject(this);
        }
    }

}