
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

        [SerializeField] private List<TurretSensor> turretList = new List<TurretSensor>();

        [Button]
        public void AddTurret(TurretSensor turretSensor)
        {
            if (turretList.Count >= maxTurrets)
            {
                Debug.Log("Max turret limit reached!");
                return;
            }

            var turret = Instantiate(
                turretSensor.gameObject,
                transform.position + stackDirection * stackOffset * turretList.Count,
                Quaternion.identity,
                transform
            ).GetComponent<TurretSensor>();

            if (stackTop != null)
            {
                stackTop.localPosition += stackDirection * stackOffset;
            }

            turretList.Add(turret);
        }

        [Button]
        public void RemoveTurret()
        {
            if (turretList.Count == 0)
            {
                Debug.Log("No turrets to remove!");
                return;
            }

            var turretToRemove = turretList[turretList.Count - 1];
            DestroyImmediate(turretToRemove.gameObject);
            turretList.RemoveAt(turretList.Count - 1);

            if (stackTop != null)
            {
                stackTop.transform.localPosition -= stackDirection * stackOffset;
            }
        }

        [Button]
        public void AddCannon() => AddTurret(TurretOptions[0]);

        [Button]
        public void AddShotter() => AddTurret(TurretOptions[1]);

        [Button]
        public void AddFlameThrower() => AddTurret(TurretOptions[2]);
    }
}
