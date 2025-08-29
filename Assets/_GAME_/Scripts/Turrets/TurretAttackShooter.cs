using GabrielBertasso;
using UnityEngine;
using UnityEngine.Events;
using SSBHealthSystem;
using System.Collections.Generic;

namespace Turret
{
    public class TurretAttackShooter : TurretAttack
    {
        [SerializeField] private Bullet bulletPrefab;
        [SerializeField] private UnityEvent OnAttack;
        [SerializeField] protected List<Transform> shootPoint;
        private int ShotPointIndex
        {
            get => shotPointIndex;
            set
            {
                if (value >= shootPoint.Count)
                {
                    shotPointIndex = 0;
                }
                else
                {
                    shotPointIndex = (value % shootPoint.Count + shootPoint.Count) % shootPoint.Count;
                }
            }
        }
        private int shotPointIndex = 0;

        protected override void Attack()
        {
            OnAttack?.Invoke();
            Bullet bullet = Instantiate(bulletPrefab.gameObject, shootPoint[ShotPointIndex].position, Quaternion.identity).GetComponent<Bullet>();
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

            bullet.SetUpCurvedMovement(shootPoint[ShotPointIndex], sensor.CurrentTarget.transform, onReachDestination);

            ShotPointIndex++;
        }
    }
}
