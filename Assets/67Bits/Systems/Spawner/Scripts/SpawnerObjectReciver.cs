using System;
using UnityEngine;

namespace SSB.Spawner
{
    public class SpawnerObjectReciver : MonoBehaviour
    {
        public Action OnDisabled;
        private void OnDisable()
        {
            OnDisabled?.Invoke();
            Destroy(this);
        }
    }
}
