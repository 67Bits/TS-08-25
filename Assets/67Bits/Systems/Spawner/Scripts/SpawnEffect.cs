using ObjectPooling;
using System.Collections;
using UnityEngine;

namespace SSB.Spawner
{
    public class SpawnEffect : MonoBehaviour
    {
        [SerializeField] private float _spawnEffectTime = 1;
        [SerializeField] private GameObject _spawnEffectPrefab;
        bool effectReady;
        private void OnEnable()
        {
            effectReady = !effectReady;
            if (effectReady)
            {
                var effect = _spawnEffectPrefab.InstantiateFromPool();
                effect.transform.position = transform.position;
                var wait = effect.AddComponent<SpawnerObjectReciver>();
                wait.StartCoroutine(Renable(gameObject));
                gameObject.SetActive(false);
            }
        }
        private IEnumerator Renable(GameObject obj)
        {
            yield return new WaitForSeconds(_spawnEffectTime);
            obj.SetActive(true);
        }
    }
}
