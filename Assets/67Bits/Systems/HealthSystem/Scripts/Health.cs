using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SSBHealthSystem
{
    public class Health : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool _debug;
        [SerializeField] private int _debugStartHealth;
        [SerializeField] private int _debugDamageToTake;
        [SerializeField] private int _debugHeal;

        [Header("Events")]
        public UnityEvent OnHeal;
        public UnityEvent OnTakeDamage;
        public UnityEvent OnDeath;

        public Action<float, float> OnStartBleed { get; set; }
        public Action OnHealthChange { get; set; }

        private float _maxHealth;
        private float _currentHealth;

        private float _reduceDamagedPercentage;

        private float _increasedHealth;
        private int _permanentIncreasedHealthPercentage;

        private bool _invincible;

        public float MaxHealth => _maxHealth + _increasedHealth;
        public float CurrentHealth => _currentHealth;
        public float ReduceDamagedPercentage { get => _reduceDamagedPercentage; set => _reduceDamagedPercentage = value; }
        public bool Invincible { get => _invincible; set => _invincible = value; }
        public float CurrentHealthPercentage => Mathf.InverseLerp(0, MaxHealth, CurrentHealth) * 100f;

        private void Awake()
        {
            if (_debug)
            {
                SetHealth(_debugStartHealth);
            }
        }

        public void SetHealth(int maxHealth)
        {
            _maxHealth = maxHealth;
            _maxHealth = MaxHealth; //to add upgrades

            _currentHealth = MaxHealth;
            OnHealthChange?.Invoke();
        }

        public bool TakeDamage(float damageToTake)
        {
            if (_currentHealth <= 0 || _invincible)
                return true;

            CalcReducedDamage(ref damageToTake);

            _currentHealth -= damageToTake;
            _currentHealth = Math.Max(0, _currentHealth);

            //Debug.Log($"{name} tomei {damageToTake} de dano, to com {_currentHealth} de vida");

            OnTakeDamage?.Invoke();
            OnHealthChange?.Invoke();

            if (_currentHealth <= 0)
            {
                Death();
                return true;
            }
            return false;
        }

        public void Heal(int healAmount, bool autoHeal = false)
        {
            _currentHealth += healAmount;
            _currentHealth = Math.Min(_currentHealth, MaxHealth);

            //Debug.Log($"{name} healing {healAmount}");

            if (!autoHeal)
                OnHeal?.Invoke();

            OnHealthChange?.Invoke();
        }

        private void CalcReducedDamage(ref float damageTaken)
        {
            damageTaken -= damageTaken * (.01f * _reduceDamagedPercentage);
        }

        private void Death()
        {
            OnDeath?.Invoke();
        }

        public void IncreaseHealth(int percentageToIncrease)
        {
            var healthToIncrease = _maxHealth * (0.01f * percentageToIncrease);
            _increasedHealth += healthToIncrease;
            //Debug.Log(name + " Increased  health " + _increasedHealth);
        }

        public void IncreasePermanentHealth(int percentageToIncrease)
        {
            _permanentIncreasedHealthPercentage += percentageToIncrease;
            //Debug.Log(name + " increasing health in " + _permanentIncreasedHealthPercentage + "%");
        }

        public void ResetHealthStats()
        {
            _increasedHealth = 0;
            //Debug.Log(name + " reseting health");
        }

        public void StartBleed(float bleedTimeInSeconds, float damagePerBleed)
        {
            if (_currentHealth <= 0)
                return;

            OnStartBleed?.Invoke(bleedTimeInSeconds, damagePerBleed);
        }

        public void ApplyPermanentStats()
        {
            IncreaseHealth(_permanentIncreasedHealthPercentage);
            _currentHealth = MaxHealth;
        }

        [Button]
        private void DebugDamage()
        {
            TakeDamage(_debugDamageToTake);
        }

        [Button]
        private void DebugHeal()
        {
            Heal(_debugHeal);
        }
        #region Cheats

        [ContextMenu("Kill")]
        public void TestKill()
        {
            TakeDamage(100000);
        }

        public void TurnInvincible()
        {
            _invincible = true;
        }
        #endregion
    }
}