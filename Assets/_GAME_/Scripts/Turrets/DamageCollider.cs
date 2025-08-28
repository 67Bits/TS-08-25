using SSBHealthSystem;
using UnityEngine;

namespace Turret
{
    [RequireComponent(typeof(Collider))]
    public class DamageCollider : MonoBehaviour
    {
        [SerializeField] private LayerMask targetLayer;
        private float damage;

        private void OnTriggerEnter(Collider other)
        {
            // Check if the other object is in the target layer
            if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
                return;

            // Try to get a Health component (or whatever script handles damage)
            if (other.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage);
            }
        }

        public void SetDamage(float damage)
        {
            this.damage = damage;
        }
    }
}
