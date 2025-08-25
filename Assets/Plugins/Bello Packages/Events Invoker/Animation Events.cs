using UnityEngine;
using UnityEngine.Events;

namespace Bello.EventsInvoker
{
    public class AnimationEvents : MonoBehaviour
    {
        [SerializeField] private AnimaitonEventIdentifier[] _events;
        public void Invoke(AnimationEvent eventParameters)
        {
            for (int i = 0; i < _events.Length; i++)
            {
                AnimaitonEventIdentifier @event = _events[i];
                if (eventParameters.stringParameter == @event.Id)
                {
                    @event.Event.Invoke(eventParameters);
                    break;
                }
            }
        }
        [System.Serializable]
        public class AnimaitonEventIdentifier
        {
            public string Id;
            public UnityEvent<AnimationEvent> Event;
        }
    }
}
