using UnityEngine;
using UnityEngine.Events;

namespace SSBHealthSystem
{
    public class SliderLifeBar : SliderBar
    {
        [Header("Health")]
        [SerializeField] private Health _health;

        [Header("Events")]
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private UnityEvent _onTakeDamage;
        [SerializeField] private UnityEvent _onDeath;
        [SerializeField] private UnityEvent _onAppear;
        [SerializeField] private UnityEvent _onDisappear;

        protected override void Start()
        {
            if (!_health)
            {
                Debug.LogError("No Health Assigned");
                return;
            }

            SubscribeToHealthEvents();
            base.Start();
        }

        private void SubscribeToHealthEvents()
        {
            _health.OnHealthChange += SetSliderValues;
            _health.OnHeal.AddListener(OnHeal);
            _health.OnTakeDamage.AddListener(OnTakeDamage);
            _health.OnDeath.AddListener(OnDeath);
        }

        protected override float GetTargetFill()
        {
            float current = GetCurrentValue();
            float max = GetMaxValue();

            float targetFill = current / max;
            return targetFill;
        }

        protected override string GetValueText()
        {
            float current = GetCurrentValue();
            float max = GetMaxValue();

            string text = $"{current}/{max}";

            return text;
        }

        protected override float GetCurrentValue()
        {
            return _health.CurrentHealth;
        }

        protected override float GetMaxValue()
        {
            return _health.MaxHealth;
        }

        private void OnHeal()
        {
            _onHeal?.Invoke();

            if (_autoHide)
            {
                Appear();
            }
        }

        private void OnTakeDamage()
        {
            _onTakeDamage?.Invoke();

            if (_autoHide)
            {
                Appear();
            }
        }

        private void OnDeath()
        {
            _onDeath?.Invoke();
        }

        protected override void Disappear()
        {
            _onDisappear?.Invoke();
            base.Disappear();
        }

        protected override void Appear()
        {
            _onAppear?.Invoke();
            base.Appear();
        }

    }
}