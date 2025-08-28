using System.Collections;
using UnityEngine;

namespace Turret
{
    public class TurretAttackFlame : TurretAttack
    {
        [SerializeField] private DamageCollider damageCollider;

        private Collider col;
        protected void Awake()
        {
            base.Awake();

            if (damageCollider != null)
            {
                damageCollider.SetDamage(damage);
                col = damageCollider.GetComponent<Collider>();
                col.enabled = false; // start disabled
            }
        }

        protected override void Attack()
        {
            if (damageCollider != null)
            {
                StartCoroutine(EnableColliderOneFrame());
            }
        }

        private IEnumerator EnableColliderOneFrame()
        {
            col.enabled = true;
            yield return null; // wait one frame
            col.enabled = false;
        }
    }
}
