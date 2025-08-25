using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SSBTutorial
{
    [RequireComponent(typeof(Button))]
    public class TutorialDependentButton : MonoBehaviour, ITutorial
    {
        [SerializeField] private TutorialStep _step;
        private Button _button;

        [field: SerializeField] public UnityEvent OnTutorialDisabled { get; set; }
        [field: SerializeField] public UnityEvent OnTutorialEnabled { get; set; }
        [field: SerializeField] public UnityEvent OnTutorialCompleted { get; set; }

        public void Awake()
        {
            _button = GetComponent<Button>();
            CheckStepCurrentStatus();

            _step.AddObject(this);
        }

        public void CheckStepCurrentStatus()
        {
            if (!_step)
            {
                Debug.LogWarning("Tutorial Step is Null!");
                return;
            }

            switch (_step.Status)
            {
                case TutorialStepStatus.Disabled:
                    _button.interactable = false;
                    break;
                case TutorialStepStatus.Enabled:
                    _button.interactable = true;
                    break;
                case TutorialStepStatus.Completed:
                    break;
            }
        }

        public void OnDestroy()
        {
            _step.RemoveObject(this);
        }
    }
}