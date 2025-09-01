
using SSB.Spawner;
using System.Collections;
using System.Collections.Generic;
using Turret;
using UnityEngine;
using UnityEngine.Events;

namespace TS
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private List<CombatWave> combatWaves;
        [SerializeField] private Transform parentEnemy;
        [SerializeField] private float distanceToPlayer = 10f;
        public UnityEvent OnEnemyFacePlayerStart;
        public UnityEvent OnEnemyFacePlayerEnd;
        public UnityEvent OnEnemyFacePLayerEndAfterSeconds;
        public UnityEvent OnCombatOver;
        [SerializeField] private float secondsToEndEnemyFacePlayer = 2f;

        public bool CombatOver
        {
            get
            {
                return currentSpawnerIndex + 1 >= combatWaves.Count;
            }
        }


        private int currentSpawnerIndex = 0;
        private Transform currentEnemyToFacePlayer;
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void CheckCombatOver()
        {
            if (CombatOver) OnCombatOver?.Invoke();
        }

        public void InicializeEnemyToFacePlayer()
        {
            currentEnemyToFacePlayer = Instantiate(combatWaves[currentSpawnerIndex].enemyToAmbushPlayer.gameObject, parentEnemy).transform;
            currentEnemyToFacePlayer.position = transform.position + (Vector3.forward * distanceToPlayer);
            CombatStarter combStart = currentEnemyToFacePlayer.gameObject.AddComponent<CombatStarter>();
            combStart.StartStarter(this);
            OnEnemyFacePlayerStart?.Invoke();
        }

        public void EndEnemyToFacePlayer(EnemyOrbiterAI enemy, TurretAttack turretAttack)
        {
            
            OnEnemyFacePlayerEnd?.Invoke();
            StartCoroutine(EndEnemyToFacePlayerAfterSeconds(enemy, turretAttack));
        }

        private IEnumerator EndEnemyToFacePlayerAfterSeconds(EnemyOrbiterAI enemy, TurretAttack turretAttack)
        {
            yield return new WaitForSeconds(secondsToEndEnemyFacePlayer);
            OnEnemyFacePLayerEndAfterSeconds?.Invoke();
            enemy.enabled = true;
            turretAttack.enabled = true;
        }

        public void InitializeSpawner()
        {
            if (combatWaves.Count == 0) return;
                combatWaves[currentSpawnerIndex].spawner.gameObject.SetActive(true);
        }

        [System.Serializable]
        public class CombatWave
        {
            public Spawner spawner;
            public EnemyOrbiterAI enemyToAmbushPlayer;
        }
    }
}
