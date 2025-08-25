using UnityEngine;

namespace TS
{

    public class EnemyOrbiterAI : MonoBehaviour
    {
        public string playerTag = "Player";
        public float moveSpeed = 5f;
        public float turnSpeed = 5f;
        public float minOrbitRadius = 5f;
        public float maxOrbitRadius = 10f;
        public int orbitDirection = 1;

        private Transform player;
        private float orbitRadius;
       
        void Start()
        {
            // Find player by tag
            GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObj != null)
            {
                player = playerObj.transform;

                // Random orbit radius
                orbitRadius = Random.Range(minOrbitRadius, maxOrbitRadius);

                // Random orbit direction (-1 = left, 1 = right)
                //orbitDirection = Random.value > 0.5f ? 1 : -1;
            }
        }

        void Update()
        {
            if (player == null) return;

            // Vector to player (flattened)
            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0;
            float distance = toPlayer.magnitude;

            Vector3 dirToPlayer = toPlayer.normalized;

            // Orbit direction is perpendicular to player direction
            Vector3 orbitDir = Quaternion.Euler(0, 90f * orbitDirection, 0) * dirToPlayer;

            // Adjust to keep radius (push inward or outward)
            float radiusError = distance - orbitRadius;
            Vector3 desiredDir = (orbitDir + dirToPlayer * radiusError * 0.3f).normalized;

            // Rotate smoothly toward desired direction
            Quaternion targetRot = Quaternion.LookRotation(desiredDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

            // Move forward
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

}
