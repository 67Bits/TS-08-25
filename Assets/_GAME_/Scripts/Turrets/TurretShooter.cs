using GabrielBertasso;
using UnityEngine;
using UnityEngine.Events;
using SSBHealthSystem;

namespace Turret
{
    public class TurretShooter : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField] private float shootInterval = 1f;
        [SerializeField] private float damage = 10;

        [Header("References")]
        [SerializeField] private TurretSensor sensor;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private Bullet bulletPrefab;

        private float shootTimer = 0f;

        private void Awake()
        {
            if (sensor == null)
                sensor = GetComponent<TurretSensor>();
        }

        private void Update()
        {
            if (sensor.CurrentTarget != null)
            {
                shootTimer += Time.deltaTime;
                if (shootTimer >= shootInterval)
                {
                    Shot();
                    shootTimer = 0f;
                }
            }
            else
            {
                shootTimer = 0f; // Reset timer if no target
            }
        }

        protected void Shot()
        {
            Bullet bullet = Instantiate(bulletPrefab.gameObject, shootPoint.position, Quaternion.identity).GetComponent<Bullet>();
            UnityEvent onReachDestination = new UnityEvent();

            if (sensor.CurrentTarget == null) return;
            
            Health targetHealth = sensor.CurrentTarget.GetComponent<Health>();

            if (targetHealth != null)
            {
                onReachDestination.AddListener(() =>
                {
                    if (targetHealth)
                    {
                        targetHealth.TakeDamage(damage);
                    }
                });
            }      

            bullet.SetUpCurvedMovement(shootPoint, sensor.CurrentTarget.transform, onReachDestination);
        }
    }
}
