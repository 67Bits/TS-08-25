
using System.Collections.Generic;
using UnityEngine;
using Bello;
using Sirenix.OdinInspector;
using System.Collections;

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

        public void ResetRotations(float duration = 1f)
        {
            StartCoroutine(ResetRotationsRoutine(duration));
        }

        private IEnumerator ResetRotationsRoutine(float duration)
        {
            float time = 0f;

            // Store initial rotations
            List<Quaternion> startRotations = new List<Quaternion>();
            foreach (var turret in turretList)
            {
                startRotations.Add(turret.transform.GetChild(0).rotation);
            }

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                for (int i = 0; i < turretList.Count; i++)
                {
                    if (turretList[i] == null) continue;
                    turretList[i].transform.GetChild(0).rotation = Quaternion.Slerp(startRotations[i], Quaternion.identity, t);
                }

                yield return null;
            }

            // Ensure perfect alignment at the end
            foreach (var turret in turretList)
            {
                if (turret == null) continue;
                turret.transform.GetChild(0).rotation = Quaternion.identity;
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
