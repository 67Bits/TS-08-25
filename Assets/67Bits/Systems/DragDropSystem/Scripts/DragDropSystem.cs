using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DragDrop
{
    public class DragDropSystem : MonoBehaviour
    {
        [SerializeField] private bool _limitX;
        [SerializeField] private bool _limitY;
        [SerializeField] private bool _limitZ;
        [SerializeField] private bool _clampPositionX;
        [SerializeField] private bool _clampPositionY;
        [SerializeField] private bool _clampPositionZ;
        [Space(20)]
        [SerializeField] private LayerMask _layerDragDrop;
        [SerializeField] private LayerMask _layerLines;

        [Space(10)]
        [SerializeField] private Vector3 _resetPosition;
        [SerializeField] private Transform _limitTop;
        [SerializeField] private Transform _limitBottom;
        [Space(10)]
        [SerializeField] private Transform _parentEffectTransform;
        [SerializeField] private Transform _effectStartTransform;
        [SerializeField] private Transform _effectTransform;
        [SerializeField] private UnityEvent _onDrag;
        [SerializeField] private UnityEvent _onDrop;
        [SerializeField] private UnityEvent _onSelected;
        [SerializeField] private UnityEvent _onUnSelected;


        private bool _dragging;
        private DragDrop _lastInteractedObject;
        private Collider _colliderDetector;
        private Vector3 _currentPosition;
        private DragDrop _currentTargetDragDrop;
        private GameObject _currentTarget;
        private Transform _selectedPath;
        private Vector3 _newPosition;


        void Start()
        {
            _effectTransform.SetParent(null);
            _effectStartTransform.SetParent(null);
            _colliderDetector = GetComponent<Collider>();
        }

        public void CancelDragDrop()
        {
            if (_currentTargetDragDrop != null)
            {
                _currentTargetDragDrop.OnEndDrag?.Invoke();
                _currentTargetDragDrop.transform.position = _effectStartTransform.transform.position;
                _currentTargetDragDrop = null;
                _currentTarget = null;
            }

            if (_lastInteractedObject != null)
            {
                _lastInteractedObject.OnUnSelected?.Invoke();
                _lastInteractedObject = null;
            }

            _effectStartTransform.gameObject.SetActive(false);
            _effectTransform.gameObject.SetActive(false);
            _lastInteractedObject = null;

            _dragging = false;
            _colliderDetector.enabled = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!_dragging || other.gameObject == _currentTarget) return;

            if (other.TryGetComponent(out DragDrop character))
            {
                if (_lastInteractedObject != null)
                {
                    _lastInteractedObject.OnUnSelected?.Invoke();
                    _onUnSelected?.Invoke();
                }

                _lastInteractedObject = character;
                _lastInteractedObject.OnSelected?.Invoke();
                _onSelected?.Invoke();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!_dragging) return;

            if (other.TryGetComponent(out DragDrop character) && _lastInteractedObject == character)
            {
                _lastInteractedObject.OnUnSelected?.Invoke();
                _onUnSelected?.Invoke();
                _lastInteractedObject = null;
            }

        }

        private Vector3 SetLimits(Vector3 newPosition)
        {
            if (_limitX || _limitY || _limitZ)
            {
                if (_limitX)
                    newPosition.x = Mathf.Clamp(newPosition.x, _limitBottom.position.x, _limitTop.position.x);
                if (_limitY)
                    newPosition.y = Mathf.Clamp(newPosition.y, _limitBottom.position.y, _limitTop.position.y);
                if (_limitZ)
                    newPosition.z = Mathf.Clamp(newPosition.z, _limitBottom.position.z, _limitTop.position.z);
            }

            return newPosition;
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0) && _currentTargetDragDrop != null)
                Drop();

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerDragDrop))
                {
                    SetPositionDragDrop(hit.collider.gameObject);
                }
            }

            if (_dragging && GetMousePosition(out _newPosition, ref _selectedPath) && _selectedPath)
            {
                _effectTransform.position = new Vector3(
                    _clampPositionX ? _selectedPath.position.x : _parentEffectTransform.position.x,
                    _clampPositionY ? _selectedPath.position.y : _parentEffectTransform.position.y,
                    _clampPositionZ ? _selectedPath.position.z : _parentEffectTransform.position.z
                );

                _currentPosition = SetLimits(_newPosition);
                transform.position = _currentPosition;

                if (_currentTarget != null)
                    _currentTarget.transform.position = transform.position;
            }

            _effectStartTransform.gameObject.SetActive(_currentTargetDragDrop != null);
            _effectTransform.gameObject.SetActive(_currentTargetDragDrop != null);
        }

        private bool GetMousePosition(out Vector3 position, ref Transform linePath)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            position = _resetPosition;
            linePath = null;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerLines))
            {

                if (linePath == null || linePath.gameObject != hit.collider.gameObject)
                    linePath = hit.collider.gameObject.transform;

                position = hit.point;
                return true;
            }

            return false;
        }

        public void SetPositionDragDrop(GameObject target)
        {
            _currentTargetDragDrop = target.GetComponent<DragDrop>();
            _currentTargetDragDrop.OnStartDrag?.Invoke();
            _currentTarget = target;
            _lastInteractedObject = null;
            _effectStartTransform.position = target.transform.position;


            transform.position = target.transform.position;
            _colliderDetector.enabled = true;
            _onDrag?.Invoke();
            _dragging = true;
        }

        void Drop()
        {
            _dragging = false;
            _colliderDetector.enabled = false;
            SetPath();
            _onDrop?.Invoke();
        }

        public virtual void SetPath()
        {
            if (_currentTargetDragDrop != null)
            {
                if (_lastInteractedObject != null)
                {
                    _currentTargetDragDrop.OnStartDrop?.Invoke();
                    _lastInteractedObject.OnStartDrop?.Invoke();


                    _currentTargetDragDrop.transform.position = _lastInteractedObject.transform.position;
                    _lastInteractedObject.transform.position = _effectStartTransform.transform.position;

                    _currentTargetDragDrop.OnEndDrop?.Invoke();
                    _lastInteractedObject.OnEndDrop?.Invoke();


                    _lastInteractedObject = null;
                }
                else
                {
                    _currentTargetDragDrop.transform.position = _effectTransform.transform.position;
                }
                _currentTargetDragDrop.OnEndDrag?.Invoke();

            }
            _currentTargetDragDrop = null;
        }
    }
}