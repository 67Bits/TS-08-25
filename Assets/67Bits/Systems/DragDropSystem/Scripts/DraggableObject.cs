using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DragDrop
{
    public class DraggableObject : MonoBehaviour
    {
        [field: ReadOnly, SerializeField] public bool IsDragging { get; private set; }
        [SerializeField] private Vector3 offset;
        [SerializeField] private float movementSpeed = 2.5f;
        private float cameraDistance = 10f;
        private Camera mainCamera;
        private Vector3 startDragPosition;
        [FoldoutGroup("Events")] public UnityEvent OnStartDrag;
        [FoldoutGroup("Events")] public UnityEvent OnDrop;
        private void Awake()
        {
            mainCamera = Camera.main;
        }
        private async void OnDrag()
        {
            while (IsDragging)
            {
                var targetPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition + offset) + mainCamera.transform.forward * cameraDistance;
                targetPosition.y = startDragPosition.y + .5f; // Keep the y position constant to avoid moving up/down
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Vector3.Distance(targetPosition, transform.position) * Time.deltaTime);
                await Awaitable.EndOfFrameAsync();
            }
        }
        public async void ReturnDrag()
        {
            var lerp = 0f;
            var startPosition = transform.position;
            var curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            while (lerp < 1)
            {
                lerp += Time.deltaTime * movementSpeed;
                transform.position = Vector3.Lerp(startPosition, startDragPosition, curve.Evaluate(lerp));
                await Awaitable.EndOfFrameAsync();
            }
        }
        private void OnMouseDown()
        {
            if (IsDragging || !enabled) return;
            cameraDistance = Vector3.Distance(transform.position, mainCamera.transform.position);
            IsDragging = true;
            startDragPosition = transform.position;
            OnStartDrag?.Invoke();
            OnDrag();
        }
        private void OnMouseUp()
        {
            if (!IsDragging || !enabled) return;
            IsDragging = false;
            OnDrop?.Invoke();
        }
    }
}
