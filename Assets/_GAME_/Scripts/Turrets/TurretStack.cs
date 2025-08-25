using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Turret
{
    public class TurretStack : MonoBehaviour
    {
        [Header("Stack Settings")]
        [SerializeField] private Vector3 stackDirection = Vector3.up;
        [SerializeField] private float stackOffset = 1f;
        [SerializeField] private int maxTurrets = 5;

        [Header("Turret Options")]
        public List<TurretSensor> TurretOptions = new List<TurretSensor>();

        private Stack<TurretSensor> turretStack = new Stack<TurretSensor>();

        public void AddTurret(TurretSensor turretSensor)
        {
            if (turretStack.Count >= maxTurrets)
            {
                Debug.Log("Max turret limit reached!");
                return;
            }
            var turret = Instantiate(turretSensor.gameObject, transform.position + stackDirection * stackOffset * turretStack.Count, Quaternion.identity, transform);
            turretStack.Push(turret.GetComponent<TurretSensor>());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (TurretOptions.Count > 0)
                {
                    int randomIndex = Random.Range(0, TurretOptions.Count);
                    AddTurret(TurretOptions[randomIndex]);
                }
            }
        }
    }
}
