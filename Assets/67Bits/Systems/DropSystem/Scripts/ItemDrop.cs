using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSBDrops
{
    public class ItemDrop : MonoBehaviour
    {
        [SerializeField] private ItemDropUI _itemDropUIPrefab;
        [SerializeField] private RectTransform _targetScreenPos;
        [SerializeField] private Canvas _mainCanvas;

        [Header("Events")]
        [SerializeField] private UnityEvent _onCollect;

        [ContextMenu("Collect")]
        public void Collect()
        {
            var dropUIInstance = Instantiate(_itemDropUIPrefab, _mainCanvas.transform);
            dropUIInstance.GoToPosition(transform.position, _targetScreenPos.position);
            _onCollect?.Invoke();
        }
    }

}