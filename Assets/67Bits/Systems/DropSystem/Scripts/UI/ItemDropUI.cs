using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSBDrops
{
    [RequireComponent(typeof(ItemDropUIMovement))]
    public class ItemDropUI : MonoBehaviour
    {
        [SerializeField] private ItemDropUIMovement _movement;

        [Header("Events")]
        [SerializeField] private UnityEvent _onArriveAtTargetPos;

        public void GoToPosition(Vector3 worldPosOrigin, Vector2 screenPosTarget)
        {
            _movement.OnArriveOnTargetPos += CallArriveEvent;
            _movement.GoToPosition(worldPosOrigin, screenPosTarget);
        }

        private void CallArriveEvent()
        {
            _onArriveAtTargetPos?.Invoke();
        }
    }
}