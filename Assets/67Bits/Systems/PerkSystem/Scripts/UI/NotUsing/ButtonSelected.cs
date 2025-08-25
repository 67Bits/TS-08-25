using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SSBPerks
{
    public class ButtonSelected : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] UnityEvent _onSelect;
        [SerializeField] UnityEvent _onDeselect;

        public void OnDeselect(BaseEventData eventData)
        {
            _onDeselect?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            _onSelect?.Invoke();
        }
        private void OnDisable()
        {
            OnDeselect(null);
        }
    }
}