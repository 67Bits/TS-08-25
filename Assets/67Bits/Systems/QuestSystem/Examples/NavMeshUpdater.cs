using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;

namespace MZW
{
    public class NavMeshUpdater : MonoBehaviour
    {
        public NavMeshSurface navMeshSurface;
        public float updateInterval = 1.0f;

        [SerializeField] private Transform playerTransform;
        private float nextUpdateTime;

        private void OnEnable()
        {
            UpdateNavMesh();
            StartCoroutine(UpdateMeshCoroutine());
        }
        private void OnDisable()
        {
            StopAllCoroutines();
        }
        private IEnumerator UpdateMeshCoroutine()
        {
            while (true)
            {
                if (Time.time >= nextUpdateTime)
                {
                    UpdateNavMesh();
                    nextUpdateTime = Time.time + updateInterval;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        private void UpdateNavMesh()
        {
            navMeshSurface.transform.position = playerTransform.position;
            navMeshSurface.BuildNavMesh();
        }
    }
}
