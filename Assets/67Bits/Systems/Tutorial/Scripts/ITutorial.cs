using UnityEngine.Events;

namespace SSBTutorial
{
    public interface ITutorial
    {
        UnityEvent OnTutorialDisabled { get; set; }
        UnityEvent OnTutorialEnabled { get; set; }
        UnityEvent OnTutorialCompleted { get; set; }

        void CheckStepCurrentStatus();
        void Awake();
        void OnDestroy();
    }
}