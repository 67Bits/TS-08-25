using UnityEngine;
using UnityEngine.AI;
using SSBHealthSystem;


namespace TS
{

    public class EnemyOrbiterAI : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        private Transform orbitPoint;
        private Health Health;

        void Start()
        {
            Health = GetComponent<Health>();
            SetOrbitPoint();
        }

        private void OnEnable()
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }

        void Update()
        {
            agent.SetDestination(orbitPoint.transform.position);
        }

        private void SetOrbitPoint()
        {
            orbitPoint = CircularGrid.Instance.AddMovingPoint(this);

            if (Health)
            {
                Health.OnDeath.AddListener(() => CircularGrid.Instance.RemoveMovingPoint(this));
            }
        }

        private void OnDrawGizmos()
        {
            if (agent != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(agent.destination, 0.5f);
            }
        }
    }

}
