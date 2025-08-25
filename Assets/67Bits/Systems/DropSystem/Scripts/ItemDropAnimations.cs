using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSBDrops
{
    [RequireComponent(typeof(Rigidbody))]
    public class ItemDropAnimations : MonoBehaviour
    {
        [SerializeField] private Vector3 _minForce;
        [SerializeField] private Vector3 _maxForce;
        [SerializeField] private ForceMode _forceMode;

        [SerializeField] private Rigidbody _rb;

        private void Start()
        {
            ApplyJump();
        }

        [ContextMenu("Jump")]
        private void ApplyJump()
        {
            var randomX = Random.Range(_minForce.x, _maxForce.x);
            var randomY = Random.Range(_minForce.y, _maxForce.y);
            var randomZ = Random.Range(_minForce.z, _maxForce.z);
            Vector3 randomForce = new Vector3(randomX, randomY, randomZ);
            _rb.AddForce(randomForce, _forceMode);
        }
    }
}
