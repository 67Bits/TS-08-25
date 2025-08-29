using GabrielBertasso;
using UnityEngine;
using UnityEngine.Events;
using SSBHealthSystem;

namespace Turret
{
    public class TurretAttackShooter : TurretAttack
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private UnityEvent OnAttack;

        protected override void Attack()
        {
            OnAttack?.Invoke();
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
