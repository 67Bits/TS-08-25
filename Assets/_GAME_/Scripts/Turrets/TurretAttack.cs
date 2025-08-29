using GabrielBertasso;
using SSBHealthSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Turret
{
    public class TurretAttack : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField] protected float attackInterval = 1f;
        [SerializeField] protected float damage = 10;

        [Header("References")]
        [SerializeField] protected TurretSensor sensor;

        private float attackTimer = 0f;

        protected void Awake()
        {
            if (sensor == null)
                sensor = GetComponent<TurretSensor>();
        }

        private void Update()
        {
            if (sensor.CurrentTarget != null)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    Attack();
                    attackTimer = 0f;
                }
            }
            else
            {
                attackTimer = 0f; // Reset timer if no target
            }
        }

        protected virtual void Attack()
        {
            Debug.Log("Attack");
        }
    }
}
