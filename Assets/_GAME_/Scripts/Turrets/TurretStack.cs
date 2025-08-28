
using System.Collections.Generic;
using UnityEngine;
using Bello;
using Sirenix.OdinInspector;

namespace Turret
{
    public class TurretStack : Singleton<TurretStack>
    {
        [Header("Stack Settings")]
        [SerializeField] private Vector3 stackDirection = Vector3.up;
        [SerializeField] private float stackOffset = 1f;
        [SerializeField] private int maxTurrets = 5;
        [SerializeField] private Transform stackTop;

        [Header("Turret Options")]
        public List<TurretSensor> TurretOptions = new List<TurretSensor>();

        private Stack<TurretSensor> turretStack = new Stack<TurretSensor>();

        [Button]
        public void AddTurret(TurretSensor turretSensor)
        {
            if (turretStack.Count >= maxTurrets)
            {
                Debug.Log("Max turret limit reached!");
                return;
            }
            var turret = Instantiate(turretSensor.gameObject, transform.position + stackDirection * stackOffset * turretStack.Count, Quaternion.identity, transform);

            if (stackTop != null)
            {
                stackTop.localPosition = turret.transform.position + stackDirection * stackOffset;
            }

            turretStack.Push(turret.GetComponent<TurretSensor>());
        }
    }
}
