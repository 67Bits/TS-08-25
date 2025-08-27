using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TS
{
    public class GroundManager : MonoBehaviour
    {
        [Header("Ground Settings")]
        public int spawnPerDestroy = 4;    // how many pieces to spawn when one is destroyed
        public float despawnThreshold = -10f; // Z position to despawn pieces
        public List<Ground> groundPrefabs;   // list of possible prefabs
        public List<Ground> groundSpecialPrefabs;   // list of possible prefabs
        public int specialQnt = 7;
        public int initialPieces = 5;        // how many pieces in front
        public float moveSpeed = 5f;         // treadmill speed
        public Transform parent;             // parent to keep hierarchy clean

        public UnityEvent OnEndSpecial;

        private Queue<Ground> activeGrounds = new Queue<Ground>();
        private float nextSpawnZ = 0f;       // local Z position for next piece

        private bool specialNext = false;
        private int currentSpecialCount = 0;

        void Start()
        {
            // Ensure we have a parent
            if (parent == null)
            {
                parent = new GameObject("GroundParent").transform;
            }

            nextSpawnZ = 0f;

            for (int i = 0; i < initialPieces; i++)
            {
                SpawnGround();
            }
        }

        public void SetMoveSpeed(float moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }

        public void ActivateSpecialGround()
        {
            specialNext = true;
            currentSpecialCount = specialQnt;
        }

        void Update()
        {
            // Move the parent backwards (moves all children together)
            parent.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);

            // Check if first piece is behind the player
            if (activeGrounds.Count > 0)
            {
                Ground first = activeGrounds.Peek();

                // Convert local position to world position for checking
                Vector3 worldPos = parent.TransformPoint(first.transform.localPosition);

                if (worldPos.z + first.length < despawnThreshold) // despawn threshold
                {
                    // Remove old piece
                    Ground old = activeGrounds.Dequeue();
                    Destroy(old.gameObject);

                    // Spawn new one at the end
                    for (int i = 0; i < spawnPerDestroy; i ++)
                        SpawnGround();
                }
            }
        }

        private void SpawnGround()
        {
            // Pick a random prefab
            Ground prefab;

            if (specialNext)
            {
                prefab = groundSpecialPrefabs[Random.Range(0, groundSpecialPrefabs.Count)];
                currentSpecialCount--;
                if (currentSpecialCount <= 0)
                {
                    specialNext = false;
                    OnEndSpecial.Invoke();
                }

            }
            else
            {
                prefab = groundPrefabs[Random.Range(0, groundPrefabs.Count)];
            }

                // Spawn at local position relative to parent
                Vector3 localSpawnPos = new Vector3(0f, 0f, nextSpawnZ);

            // Instantiate as child of parent with local position
            Ground newGround = Instantiate(prefab, parent);
            newGround.transform.localPosition = localSpawnPos;
            newGround.transform.localRotation = Quaternion.identity;

            // Update spawn point for next piece
            nextSpawnZ += newGround.length;

            activeGrounds.Enqueue(newGround);
        }
    }
}
