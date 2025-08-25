using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSBTutorial
{
    public class TutorialStepSetter : MonoBehaviour
    {
        [SerializeField] private TutorialStep _stepToSet;
        [SerializeField, Tooltip("Should this object activate at the Start of the scene?")] private bool _activeAtStart;

        private void Start()
        {
            if (_activeAtStart)
            {
                ActiveTutorialStep();
            }
        }

        public void ActiveTutorialStep()
        {
            if (!_stepToSet)
            {
                Debug.LogWarning("Tutorial Step is Null!");
                return;
            }
            _stepToSet.ActiveTutorial();
        }

        public void CompleteTutorialStep()
        {
            if (!_stepToSet)
            {
                Debug.LogWarning("Tutorial Step is Null!");
                return;
            }
            _stepToSet.CompleteTutorial();
        }

        public void DisableTutorialStep()
        {
            if (!_stepToSet)
            {
                Debug.LogWarning("Tutorial Step is Null!");
                return;
            }
            _stepToSet.DisableTutorial();
        }
    }

}