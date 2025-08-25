using UnityEngine;
using UnityEngine.Events;

namespace SSBLevelProgression
{
    public class LevelEventWatcher : MonoBehaviour
    {
        [SerializeField] private LevelEvent _eventToWatch;
        [SerializeField] private UnityEvent _onEventAchieved;

        private void OnEnable()
        {
            SubscribeToEvent();
        }

        private void SubscribeToEvent()
        {
            _eventToWatch.AddWatcher(this);
        }

        private void UnsubscribeToEvent()
        {
            _eventToWatch.RemoveWatcher(this);
        }

        public void OnEventAchieved()
        {
            _onEventAchieved.Invoke();
        }

        private void OnDestroy()
        {
            UnsubscribeToEvent();
        }
    }

}