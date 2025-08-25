using UnityEngine;
using UnityEngine.Events;

namespace DragDrop
{
    [RequireComponent(typeof(Collider))]
    public class DragDrop : MonoBehaviour
    {
        [Header("Drag")]
        public UnityEvent OnStartDrag;
        public UnityEvent OnEndDrag;

        [Header("Drop")]
        public UnityEvent OnStartDrop;
        public UnityEvent OnEndDrop;

        [Header("Selection")]
        public UnityEvent OnSelected;
        public UnityEvent OnUnSelected;
    }
}