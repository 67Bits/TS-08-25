using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.EnergySpell
{
    public class ManaGenerator : MonoBehaviour
    {
        public static ManaGenerator Instance { get; private set; }


        [SerializeField] private bool _autoInitialize = true;
        [Header("Mana Settings")]
        [SerializeField] private float _maxMana;
        [SerializeField] private float _manaPerSecond;
        [field: SerializeField, ReadOnly] public float Mana { get; private set; }

        // Events
        public Action<float> OnManaUpdate;
        public UnityEvent OnManaAdded;
        public UnityEvent OnManaRemoved;

        Awaitable currentUpdateRoutine;
        private void Awake()
        {
            Instance = this;
            if(_autoInitialize) Initialize();
        }
        [Button]
        public void Initialize()
        {
            currentUpdateRoutine?.Cancel();
            currentUpdateRoutine = VirtualFixedUpdate();
        }
        private async Awaitable VirtualFixedUpdate()
        {
            while (enabled)
            {
                Mana += Time.fixedDeltaTime * _manaPerSecond;
                Mana = Mathf.Clamp(Mana, 0 , _maxMana);
                OnManaUpdate?.Invoke(Mana);
                await Awaitable.FixedUpdateAsync();
            }
        }
        public void AddMana(float value)
        {
            Mana += value;
            OnManaAdded?.Invoke();
        }
        public void RemoveMana(float value)
        {
            Mana -= value;
            OnManaRemoved?.Invoke();
        }

    }
}
