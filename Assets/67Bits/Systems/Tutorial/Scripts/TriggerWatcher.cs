using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSBTutorial
{
    public class TriggerWatcher : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onTriggerEvent;
        private void OnTriggerEnter(Collider other)
        {
            _onTriggerEvent?.Invoke();
        }
    }
}
