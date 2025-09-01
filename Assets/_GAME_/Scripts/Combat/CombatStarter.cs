using SSBHealthSystem;
using UnityEngine;
using Turret;
namespace TS
{
    public class CombatStarter : MonoBehaviour
    {
        private CombatManager combatManager;

        private Transform playerTransform;
        private float distanceToPlayer = 7f;
        private EnemyOrbiterAI enemyAI;
        private Health health;
        private TurretAttack turretAttack;

        private void Awake()
        {
            gameObject.layer = 0;
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            enemyAI = GetComponent<EnemyOrbiterAI>();
            health = GetComponent<Health>();
            turretAttack = GetComponentInChildren<TurretAttack>();
            turretAttack.enabled = false;
            enemyAI.enabled = false;
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= distanceToPlayer)
            {
                combatManager.EndEnemyToFacePlayer(enemyAI, turretAttack);
                Destroy(this);
            }
        }

        public void StartStarter(CombatManager combatManager)
        {
            this.combatManager = combatManager;
        }
    }
}
