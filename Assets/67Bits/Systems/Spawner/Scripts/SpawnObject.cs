using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace SSB.Spawner
{
    public class SpawnObject : MonoBehaviour
    {
        [ReadOnly] public Spawner Spawner;
        [ReadOnly] public string WaveName;
        public Action OnCreated;
        public Action OnDisabled;
        private bool _destroyed = false;
        public void OnDisable()
        {
            if (_destroyed) return;
            _destroyed = true;
            OnDisabled?.Invoke();
            Spawner.Waves.FirstOrDefault((wave) => wave.Name == WaveName).RemoveSpawn();
            Destroy(this);
        }
    }
}
