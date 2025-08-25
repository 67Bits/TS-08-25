using UnityEngine;
using UnityEngine.Events;

namespace Bello.EventsInvoker
{
    public class OnEnableEvents : MonoBehaviour
    {
        public UnityEvent OnEnableEvent;
        public UnityEvent OnDisableEvent;
        private void OnEnable() => OnEnableEvent?.Invoke();
        private void OnDisable() => OnDisableEvent?.Invoke();
    }
}
